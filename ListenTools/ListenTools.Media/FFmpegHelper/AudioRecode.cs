using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using ListenTools.Media.MediaModels;

namespace ListenTools.Media.FFmpegHelper;

internal unsafe class AudioRecode
{
     /// <summary>
    /// PTS最大值,超过这个值直接归零
    /// </summary>
    private const long MaxValue = 9223372036854700000;
    private Thread _ffmpegEncoderThread;
    private bool _encoderStatus = false;
    private ConcurrentQueue<WaveData> _bytesCollection;
    private AVFrame* _frame;
    private AVStream* _outSteam;
    
    //private const string OutputFile = @"D:\aaa.aac";
    //private AVFormatContext* _outputFormatContext;
    private AVCodecContext* _encoderContext;
    private SwrContext* _swrContext;
    private readonly SampleInfo _inputInfo;

    internal void AddWaveData(WaveData wavData) => _bytesCollection.Enqueue(wavData);

    internal AudioRecode(SampleInfo inputSample)
    {
        this._inputInfo = inputSample;
        // _tcpClient = tcpClient;
    }

    internal void InitEncoder()
    {
        _bytesCollection = new ConcurrentQueue<WaveData>();
        int error = 0;

        // 查找AAC音频编解码器
        var aacEncoder = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_AAC);
        if (aacEncoder == null)
        {
            Console.WriteLine("系统找不到音频编码器");
            return;
        }

        _encoderContext = ffmpeg.avcodec_alloc_context3(aacEncoder);
        _encoderContext->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
        _encoderContext->channels = _inputInfo.Channels; //双通道
        _encoderContext->channel_layout = _inputInfo.ChannelLayout;
        _encoderContext->sample_rate = _inputInfo.SampleRate; //48000; //采样率
        _encoderContext->sample_fmt = aacEncoder->sample_fmts[0];
        _encoderContext->bit_rate = _inputInfo.BitRate; //192000;
        _encoderContext->time_base = new AVRational { num = 1, den = _inputInfo.SampleRate };

        error = ffmpeg.avcodec_open2(_encoderContext, aacEncoder, null);
        if (error < 0)
        {
            Console.WriteLine($"系统无法打开编码器:{error}");
            return;
        }

        // 输出

        #region 创建输出(因为我们要把重采样的数据通过tcp发出去，不需要额外输出到文件，所以这里直接注释掉，后买调试需要录制音频到本地文件可以再放开)

        // AVFormatContext* outputFormatContext;
        // error = ffmpeg.avformat_alloc_output_context2(&outputFormatContext, null, null, OutputFile);
        //
        // if (error < 0)
        // {
        //     Console.WriteLine($"avformat_alloc_output_context2 error : {error}");
        //     return;
        // }
        //
        // if (outputFormatContext == null)
        // {
        //     Console.WriteLine("无法创建输出上下文");
        //     return;
        // }
        //
        // _outSteam = ffmpeg.avformat_new_stream(outputFormatContext, aacEncoder);
        // if (_outSteam == null)
        // {
        //     Console.WriteLine("无法创建输出流");
        //     return;
        // }
        //
        // _outSteam->codecpar->codec_tag = 0;
        //
        // error = ffmpeg.avcodec_parameters_from_context(_outSteam->codecpar, _encoderContext);
        // if (error < 0)
        // {
        //     Console.WriteLine($"无法从编码器上下文拷贝参数:{error}");
        //     return;
        // }
        //
        // ffmpeg.av_dump_format(outputFormatContext, 0, OutputFile, 1);
        // error = ffmpeg.avio_open(&outputFormatContext->pb, OutputFile, ffmpeg.AVIO_FLAG_WRITE);
        // if (error < 0)
        // {
        //     Console.WriteLine($"无法打开输出文件:{error}");
        //     return;
        // }
        //
        // error = ffmpeg.avformat_write_header(outputFormatContext, null);
        // if (error < 0)
        // {
        //     Console.WriteLine($"无法写入输出数据头:{error}");
        //     return;
        // }
        //
        // _outputFormatContext = outputFormatContext;

        #endregion

        #region 重采样上下文

        _swrContext = ffmpeg.swr_alloc();
        ffmpeg.av_opt_set_int(_swrContext, "in_channel_layout",
            ffmpeg.av_get_default_channel_layout(_inputInfo.Channels), 0);
        ffmpeg.av_opt_set_int(_swrContext, "in_sample_rate", _inputInfo.SampleRate, 0);
        ffmpeg.av_opt_set_sample_fmt(_swrContext, "in_sample_fmt", _inputInfo.SampleFmt, 0);

        ffmpeg.av_opt_set_int(_swrContext, "out_channel_layout",
            ffmpeg.av_get_default_channel_layout(_inputInfo.Channels), 0);
        ffmpeg.av_opt_set_int(_swrContext, "out_sample_rate", _encoderContext->sample_rate, 0);
        ffmpeg.av_opt_set_sample_fmt(_swrContext, "out_sample_fmt", _encoderContext->sample_fmt, 0);

        error = ffmpeg.swr_init(_swrContext);
        if (error < 0)
        {
            Console.WriteLine($"ffmpeg.swr_init error : {error}");
            return;
        }

        #endregion

        _frame = ffmpeg.av_frame_alloc();
        _frame->format = (int)_encoderContext->sample_fmt; //NAudio采集的编码格式是FLT格式
        _frame->channel_layout = _encoderContext->channel_layout;
        _frame->sample_rate = _encoderContext->sample_rate;
        _frame->nb_samples = _encoderContext->frame_size;
        error = ffmpeg.av_frame_get_buffer(_frame, 0);
        if (error < 0)
        {
            Console.WriteLine($"av_frame_get_buffer error : {error}");
            return;
        }
    }

    internal void BeginConverter()
    {
        if (_ffmpegEncoderThread != null)
        {
            if (_encoderStatus)
                _encoderStatus = false;
            _ffmpegEncoderThread.Join(5000);
            if (_ffmpegEncoderThread.ThreadState != ThreadState.Aborted)
                _ffmpegEncoderThread.Abort();
            _ffmpegEncoderThread = null;
        }

        _ffmpegEncoderThread = new Thread(ReSampling);
        _ffmpegEncoderThread.IsBackground = true;
        _encoderStatus = true;
        _ffmpegEncoderThread.Start();
    }

    internal void EndConverter()
    {
        if (_ffmpegEncoderThread != null)
        {
            _encoderStatus = false;
            _ffmpegEncoderThread.Join();
            _ffmpegEncoderThread = null;
        }

        if (_bytesCollection.Any())
        {
            _bytesCollection = new ConcurrentQueue<WaveData>();
        }

        if (_frame != null)
        {
            ffmpeg.av_frame_unref(_frame);
            var frame = _frame;
            ffmpeg.av_frame_free(&frame);
            _frame = null;
        }

        if (_encoderContext != null)
        {
            ffmpeg.avcodec_close(_encoderContext);

            var codec = _encoderContext;
            ffmpeg.avcodec_free_context(&codec);
            _encoderContext = null;
        }

        #region 写入到文件

        // if (_outputFormatContext != null)
        // {
        //     if ((_outputFormatContext->oformat->flags & ffmpeg.AVFMT_NOFILE) == 0)
        //         ffmpeg.avio_close(_outputFormatContext->pb);
        //     ffmpeg.avformat_free_context(_outputFormatContext);
        // }

        #endregion


        _outSteam = null;
    }

    /// <summary>
    /// 开始重采样
    /// </summary>
    private void ReSampling()
    {
        int error = 0;
        byte[] moreDt = null;
        long framePts = 0;
        while (_encoderStatus)
        {
            if (!_bytesCollection.TryDequeue(out var wavData))
                continue;

            var data = wavData.Data;
            var length = wavData.DataLength;

            var packet = ffmpeg.av_packet_alloc();
            // 计算输入源每一帧大小,因为录制过来的数据包大小不一致，所以这里多加一个循环用于固定输入包
            var inputSampleCount = _frame->nb_samples *
                                   ffmpeg.av_get_bytes_per_sample(AVSampleFormat.AV_SAMPLE_FMT_FLT) *
                                   _frame->channels;
            int offset = 0;
            while (offset < length)
            {
                var readLen = Math.Min(length - offset, inputSampleCount);

                if (readLen < inputSampleCount)
                {
                    moreDt = new byte[readLen];
                    Buffer.BlockCopy(data, offset, moreDt, 0, readLen);
                    break;
                }

                var bt = new byte[readLen];
                if (moreDt != null)
                {
                    Buffer.BlockCopy(moreDt, 0, bt, 0, moreDt.Length);
                    readLen = readLen - moreDt.Length;
                    Buffer.BlockCopy(data, offset, bt, moreDt.Length, readLen);
                    moreDt = null;
                }
                else
                    Buffer.BlockCopy(data, offset, bt, 0, readLen);

                offset += readLen;
                fixed (byte* btPtr = bt)
                {
                    var len = ffmpeg.swr_convert(_swrContext,
                        _frame->extended_data, _encoderContext->frame_size,
                        &btPtr, _frame->nb_samples);

                    _frame->pts = framePts * 100;
                    error = ffmpeg.avcodec_send_frame(_encoderContext, _frame);
                    if (error != 0)
                        continue;
                    error = ffmpeg.avcodec_receive_packet(_encoderContext, packet);
                    if (error != ffmpeg.AVERROR(ffmpeg.EAGAIN) && error != ffmpeg.AVERROR_EOF)
                    {
                        #region 下面两个都可以写入到文件

                        //error = ffmpeg.av_write_frame(_outputFormatContext, packet);
                        //error = ffmpeg.av_interleaved_write_frame(_outputFormatContext, packet);

                        #endregion

                        var head = Aac.GenerateAdtsHeader(_frame->sample_rate,_frame->channels, packet->size);
                        var afterData = new byte[packet->size + 7];
                        // 填充7个字节的ADTS头
                        Buffer.BlockCopy(head, 0, afterData, 0, 7);
                        // 填充AAC数据
                        Marshal.Copy((IntPtr)packet->data, afterData, 0, afterData.Length);
                        //_tcpClient?.WriteData(afterData);
                        
                        if (error < 0)
                        {
                            Console.WriteLine($"av_interleaved_write_frame : {error}");
                        }
                    }

                    ffmpeg.av_packet_unref(packet);
                }

                if ((framePts + 1) * 100 >= MaxValue)
                    framePts = 0;

                framePts++;
            }

            ffmpeg.av_packet_unref(packet);
            ffmpeg.av_packet_free(&packet);
        }
    }

    public void Dispose()
    {
        EndConverter();
    }
}
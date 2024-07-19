using FFmpeg.AutoGen;
using ListenTools.Media.FFmpegHelper;
using ListenTools.Media.MediaModels;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace ListenTools.Media.AudioHelper;

/// <summary>
/// 捕获设备输出音频
/// </summary>
public class CaptureOutputAudio : IDisposable
{
    public delegate void CaptureStatusChangedDelegate(bool status);

    public event CaptureStatusChangedDelegate? CaptureStatusChanged;
    private MMDevice? _currentCaptureDevice;
    private WasapiLoopbackCapture? _loopbackCapture;
    private bool _captureStatus;

    /// <summary>
    /// 音频编码器
    /// </summary>
    private AudioRecode? _audioEncode;


    public CaptureOutputAudio()
    {
        _currentCaptureDevice = WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice();
        _loopbackCapture = new WasapiLoopbackCapture(_currentCaptureDevice);
        _loopbackCapture.DataAvailable += LoopbackCaptureOnDataAvailable;
        _loopbackCapture.RecordingStopped += LoopbackCaptureOnRecordingStopped;
        var mixFormat = _loopbackCapture.WaveFormat; // _currentCaptureDevice.AudioClient.MixFormat;
        SampleInfo audioInfo = new(mixFormat.Channels,
            AVSampleFormat.AV_SAMPLE_FMT_FLT,
            mixFormat.SampleRate,
            mixFormat.AverageBytesPerSecond);
        _audioEncode = new AudioRecode(audioInfo);
        _audioEncode.InitEncoder();
    }

    public void BeginCapture()
    {
        _loopbackCapture?.StartRecording();
        _audioEncode?.BeginConverter();
    }

    public void EndCapture()
    {
        if (_loopbackCapture?.CaptureState == CaptureState.Capturing)
            _loopbackCapture.StopRecording();
    }

    /// <summary>
    /// 录制结束
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoopbackCaptureOnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        _captureStatus = false;
        CaptureStatusChanged?.Invoke(_captureStatus);
    }

    /// <summary>
    /// 接收到录制音频的数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoopbackCaptureOnDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (!_captureStatus)
        {
            _captureStatus = true;
            CaptureStatusChanged?.Invoke(_captureStatus);
        }

        _audioEncode?.AddWaveData(new WaveData { Data = e.Buffer, DataLength = e.BytesRecorded });
    }


    public void Dispose()
    {
        if (_audioEncode != null)
        {
            _audioEncode.Dispose();
            _audioEncode = null;
        }

        if (_currentCaptureDevice != null)
        {
            _currentCaptureDevice.Dispose();
            _currentCaptureDevice = null;
        }

        if (_loopbackCapture != null)
        {
            if (_loopbackCapture.CaptureState == CaptureState.Capturing)
                _loopbackCapture.StopRecording();
            _loopbackCapture.DataAvailable -= LoopbackCaptureOnDataAvailable;
            _loopbackCapture.RecordingStopped -= LoopbackCaptureOnRecordingStopped;
            _loopbackCapture.Dispose();
            _loopbackCapture = null;
        }
    }
}
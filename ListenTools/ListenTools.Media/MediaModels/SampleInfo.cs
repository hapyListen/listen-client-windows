using FFmpeg.AutoGen;

namespace ListenTools.Media.MediaModels;

internal class SampleInfo(
    int channels,
    AVSampleFormat sampleFmt,
    int sampleRate,
    int bitRate)
{

    /// <summary>
    /// 输入的通道
    /// </summary>
    public int Channels { get; } = channels;

    /// <summary>
    /// 通道类型
    /// </summary>
    public ulong ChannelLayout { get; } = (ulong)ffmpeg.av_get_default_channel_layout(channels);

    /// <summary>
    /// 输入的采样格式
    /// </summary>
    public AVSampleFormat SampleFmt { get; } = sampleFmt;

    /// <summary>
    /// 输入的采样率
    /// </summary>
    public int SampleRate { get; } = sampleRate;

    public int BitRate { get; } = bitRate;
}
using System.Runtime.CompilerServices;
using FFmpeg.AutoGen;

namespace ListenTools.Media;

public static class Instance
{
    /// <summary>
    /// 初始化加载FFmpeg环境
    /// </summary>
    public static void Init()
    {
        InitFfmpegPath();
    }
    
    
    private static void InitFfmpegPath()
    {
        string libsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs", "ffmpeg", "bin",
            Environment.Is64BitProcess ? "x64" : "x86");
        if (!System.IO.Directory.Exists(libsPath))
            throw new FileNotFoundException("libs lost");

        ffmpeg.RootPath = libsPath;

        // 日志打印等级
        ffmpeg.av_log_set_level(ffmpeg.AV_LOG_INFO);
        // 注册所有输入输出设备
        ffmpeg.avdevice_register_all();
    }
}
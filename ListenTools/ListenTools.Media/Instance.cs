using System.Diagnostics;
using System.Runtime.CompilerServices;
using FFmpeg.AutoGen;
using ListenTools.Media.Net;

namespace ListenTools.Media;

public static class Instance
{
    /// <summary>
    /// 初始化加载FFmpeg环境
    /// </summary>
    public static void Init()
    {
        //InitFfmpegPath();
    }

    public static void TcpClientTest()
    {
        // TcpClient client = new TcpClient("127.0.0.1", 11231);
        // client.OnConnectionStatusChanged += (tcp, status) =>
        // {
        //     if (status)
        //     {
        //         Debug.WriteLine("Tcp 连接成功");
        //         tcp.BeginReceive();
        //     }
        //     else 
        //         Debug.WriteLine("Tcp 连接已断开");
        // };
        // client.BeginConnect();
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
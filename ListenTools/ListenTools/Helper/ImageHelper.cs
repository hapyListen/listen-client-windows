using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ListenTools.Helper;

public static partial class ImageHelper
{
    public enum PathType
    {
        Invalid = 0,
        Local = 1,
        Http = 2,
        Resources = 3
    }

    public enum ImageFormat
    {
        Invalid = 0,
        Gif = 7173,
        Jpg = 255216,
        Png = 13780,
        Bmp = 6677
    }


    #region 正则匹配

    private const string LocalRegex = @"^([C-J]):\\([^:&]+\\)*([^:&]+).(jpg|jpeg|png|gif|bmp)$";
    private const string HttpRegex = @"^((https|http):\/\/)?([^\\*+@]+)$";

    [GeneratedRegex(LocalRegex, RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex LocalImageRegex();

    [GeneratedRegex(HttpRegex, RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex HttpImageRegex();

    #endregion

    private static readonly ConcurrentDictionary<string, Bitmap?> GlobalImageCache = new();

    private static readonly LimitedConcurrencyLevelTaskScheduler LoadImageTaskScheduler = new(10);

    public static ImageFormat GetImageType(byte[]? bytes)
    {
        var type = ImageFormat.Invalid;
        if (bytes == null || bytes.Length < 2)
            return type;
        try
        {
            var fileHead = Convert.ToInt32($"{bytes[0]}{bytes[1]}");
            if (!Enum.IsDefined(typeof(ImageFormat), fileHead))
                type = ImageFormat.Invalid;
            else
                type = (ImageFormat)fileHead;
        }
        catch (Exception ex)
        {
            type = ImageFormat.Invalid;
            Console.WriteLine($"获取图片类型失败 {ex.Message}");
        }

        return type;
    }

    public static PathType ValidatePathType(string path)
    {
        if (string.IsNullOrEmpty(path))
            return PathType.Invalid;
        if (path.StartsWith("avares://"))
            return PathType.Resources;
        else if (LocalImageRegex().IsMatch(path))
            return PathType.Local;
        else if (HttpImageRegex().IsMatch(path))
            return PathType.Http;
        else
            return PathType.Invalid;
    }

    public static async Task<Bitmap?> GetImageData(string url)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        if (GlobalImageCache.TryGetValue(url, out var cache))
            return cache;

        var pathType = ValidatePathType(url);

        return pathType switch
        {
            PathType.Local => LoadBytesFromLocal(url),
            PathType.Resources => LoadBytesFromApplicationResource(url),
            PathType.Http => await Task.Factory.StartNew(() => LoadBytesFromHttp(url), CancellationToken.None,
                TaskCreationOptions.None, LoadImageTaskScheduler),
            _ => null
        };
    }

    /// <summary>
    /// 从网络下载图片
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static Bitmap? LoadBytesFromHttp(string url)
    {
        try
        {
            Console.WriteLine($"下载网络资源图片：{url}");
            var httpFactory = Global.GlobalContext.Instance.GetHttpFactory();
            using var client = httpFactory.CreateClient();
            var data = client.GetByteArrayAsync(url).Result;
            if (data.Any())
            {
                Console.WriteLine($"下载完成：{url}");
            }

            return AnalysisBytes(url, data);
        }
        catch
        {
            // ignored
        }

        return null;
    }

    /// <summary>
    /// 加载本地资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static Bitmap? LoadBytesFromLocal(string path)
    {
        if (!System.IO.File.Exists(path))
            return null;

        try
        {
            return AnalysisBytes(path, File.ReadAllBytes(path));
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// 加载AvaloniaResource资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static Bitmap? LoadBytesFromApplicationResource(string path)
    {
        try
        {
            using var streamInfo = AssetLoader.Open(new Uri(path));
            var bytes = new byte[streamInfo.Length];
            _ = streamInfo.Read(bytes, 0, bytes.Length);
            return AnalysisBytes(path, bytes);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private static Bitmap? AnalysisBytes(string url, byte[] data)
    {
        var imgType = GetImageType(data);
        if (imgType == ImageFormat.Invalid)
        {
            Console.WriteLine("ImageFormat Invalid");
            return null;
        }

        using var memStream = new MemoryStream(data);
        var bitMap = new Bitmap(memStream);
        GlobalImageCache.TryAdd(url, bitMap);
        return bitMap;
    }
}
using System;
using System.Text.RegularExpressions;

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
        Invalid = 0, Gif = 7173, Jpg = 255216, Png = 13780, Bmp = 6677
    }
       
    private const string LocalRegex = @"^([C-J]):\\([^:&]+\\)*([^:&]+).(jpg|jpeg|png|gif|bmp)$";
    private const string HttpRegex = @"^((https|http):\/\/)?([^\\*+@]+)$";
    
    [GeneratedRegex(LocalRegex, RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex LocalImageRegex();

    [GeneratedRegex(HttpRegex, RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex HttpImageRegex();

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
 
    
}
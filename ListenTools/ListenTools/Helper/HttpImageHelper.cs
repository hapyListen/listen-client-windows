using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace ListenTools.Helper;

public static class HttpImageHelper
{

    private static readonly TimeSpan HttpTimeout = TimeSpan.FromMilliseconds(5000);
    private static readonly ConcurrentDictionary<string, byte[]> HttpImageCache = new();

    public static async Task<byte[]> GetHttpImage(string url)
    {
        if (HttpImageCache.TryGetValue(url, out var image))
            return image;

        var httpClientFactory = GlobalContext.Instance.GetHttpFactory();
        using var client = httpClientFactory.CreateClient("ImageDownloader");
        client.Timeout = HttpTimeout;
        var imageData = await client.GetByteArrayAsync(url);
        HttpImageCache.TryAdd(url, imageData);
        return imageData;
    }
    
}
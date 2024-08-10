using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ListenTools.Global;

namespace ListenTools.Helper;

public static class HttpImageHelper
{

    private static readonly TimeSpan HttpTimeout = TimeSpan.FromMilliseconds(5000);

    public static async Task<byte[]> GetHttpImage(string url)
    {
        return await Task.Factory.StartNew(() =>
        {
            var httpClientFactory = GlobalContext.Instance.GetHttpFactory();
            using var client = httpClientFactory.CreateClient();
            client.Timeout = HttpTimeout;
            var imageData = client.GetByteArrayAsync(url).Result;
            return imageData;
        });
        
    }
    
}
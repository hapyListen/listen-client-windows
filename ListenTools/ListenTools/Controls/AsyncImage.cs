using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using AvaloniaGif;
using ListenTools.Global;
using ListenTools.Helper;

namespace ListenTools.Controls;

public class AsyncImage : TemplatedControl
{
    #region DependencyProperty

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<AsyncImage, bool>(nameof(IsLoading), defaultValue: false);

    public static readonly StyledProperty<string> UrlSourceProperty =
        AvaloniaProperty.Register<AsyncImage, string>(nameof(UrlSource));

    public static readonly StyledProperty<string> FailUrlSourceProperty =
        AvaloniaProperty.Register<AsyncImage, string>(nameof(FailUrlSource));

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<AsyncImage, Stretch>(nameof(Avalonia.Media.Stretch),
            defaultValue: Stretch.Uniform);

    #endregion

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        private set => SetValue(IsLoadingProperty, value);
    }

    public string UrlSource
    {
        get => GetValue(UrlSourceProperty);
        set => SetValue(UrlSourceProperty, value);
    }

    /// <summary>
    /// 仅限Resourcesl路径
    /// </summary>
    public string FailUrlSource
    {
        get => GetValue(FailUrlSourceProperty);
        set => SetValue(FailUrlSourceProperty, value);
    }


    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    #region Property

    private Border? _imageContainer;
    private Image? _staticImage;
    private GifImage? _gifImage;
    private ImageHelper.ImageFormat _currentImageFormat;

    private static readonly ConcurrentDictionary<string, Bitmap> ImageCacheList;
    private static readonly ConcurrentDictionary<string, Stream> GifStreamCacheList;
    private static readonly LimitedConcurrencyLevelTaskScheduler HttpTaskScheduler;

    #endregion

    static AsyncImage()
    {
        ImageCacheList = new ConcurrentDictionary<string, Bitmap>();
        GifStreamCacheList = new ConcurrentDictionary<string, Stream>();
        HttpTaskScheduler = new LimitedConcurrencyLevelTaskScheduler(10);
        UrlSourceProperty.Changed.AddClassHandler<AsyncImage>(OnUrlSourceChanged);
    }


    private static void OnUrlSourceChanged(AsyncImage sender, AvaloniaPropertyChangedEventArgs args)
    {
        sender.Load();
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _imageContainer = e.NameScope.Find<Border>("ImageContainer");
        if (_imageContainer == null)
            return;

        this.Load();
    }

    private void Load()
    {
        if (_imageContainer == null)
            return;
        if (!string.IsNullOrEmpty(UrlSource))
        {
            IsLoading = !ImageCacheList.ContainsKey(UrlSource) && !GifStreamCacheList.ContainsKey(UrlSource);

            #region 读取缓存

            if (ImageCacheList.TryGetValue(UrlSource, out var source))
            {
                SetStaticSource(source);
                return;
            }

            if (GifStreamCacheList.TryGetValue(UrlSource, out var value))
            {
                SetGifSource(value);
                return;
            }

            #endregion

            this.Load(UrlSource, FailUrlSource);
        }
    }

    private void Load(string url, string failUrl)
    {
        //解析路径类型
        var pathType = Helper.ImageHelper.ValidatePathType(url);
        if (pathType == ImageHelper.PathType.Invalid)
        {
            LoadFail(failUrl);
            return;
        }

        if (pathType == ImageHelper.PathType.Http)
            LoadHttp(url, failUrl);
        else
            LoadLocal(url, failUrl, pathType);
    }

    /// <summary>
    /// 加载失败图像
    /// </summary>
    /// <param name="failUrl"></param>
    private void LoadFail(string failUrl)
    {
        byte[]? imgBytes = null;
        var errorMessage = string.Empty;
        var pathType = ImageHelper.ValidatePathType(failUrl);
        if (pathType == ImageHelper.PathType.Invalid)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"LoadFail 无效的路径 {failUrl}");
            Console.ResetColor();
            return;
        }

        if (pathType == ImageHelper.PathType.Local)
            imgBytes = LoadBytesFromLocal(failUrl, out errorMessage);
        else if (pathType == ImageHelper.PathType.Resources)
            imgBytes = LoadBytesFromApplicationResource(failUrl, out errorMessage);


        if (string.IsNullOrEmpty(errorMessage) && imgBytes != null)
            AnalysisBytes(imgBytes, failUrl, out errorMessage);

        if (string.IsNullOrEmpty(errorMessage))
            return;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"LoadFail {errorMessage} {failUrl}");
        Console.ResetColor();
    }

    private void LoadLocal(string url, string failUrl, ImageHelper.PathType pathType)
    {
        byte[]? imgBytes = null;
        var errorMessage = string.Empty;
        if (pathType == ImageHelper.PathType.Local)
            imgBytes = LoadBytesFromLocal(url, out errorMessage);
        else if (pathType == ImageHelper.PathType.Resources)
            imgBytes = LoadBytesFromApplicationResource(url, out errorMessage);

        if (string.IsNullOrEmpty(errorMessage) && imgBytes != null)
            AnalysisBytes(imgBytes, url, out errorMessage);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            LoadFail(failUrl);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"LoadLocal {errorMessage} {url}");
            Console.ResetColor();
        }
    }

    private void LoadHttp(string url, string failUrl)
    {
        Task.Factory.StartNew(() =>
        {
            var errorMessage = string.Empty;
            var imgBytes = LoadBytesFromHttp(url);

            if (imgBytes == null)
                errorMessage = "Download image failed";
            else
                AnalysisBytes(imgBytes, url, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                LoadFail(failUrl);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"LoadHttp: {errorMessage}, {url}");
                Console.ResetColor();
            }

            return Task.CompletedTask;
        }, CancellationToken.None, TaskCreationOptions.None, HttpTaskScheduler);
    }

    private void AnalysisBytes(byte[]? imgBytes, string cacheKey, out string errorMessage)
    {
        errorMessage = string.Empty;

        #region 读取文件类型

        var imgType = Helper.ImageHelper.GetImageType(imgBytes);
        if (imgType == ImageHelper.ImageFormat.Invalid)
        {
            imgBytes = null;
            errorMessage = "Invalid ImageFile";
            return;
        }

        _currentImageFormat = imgType;

        #endregion

        #region 加载图像

        if (imgType != ImageHelper.ImageFormat.Gif)
        {
            //加载静态图像    
            var imgSource = LoadStaticImage(cacheKey, imgBytes, out errorMessage);
            if (imgSource == null)
                return;
            SetStaticSource(imgSource);
        }
        else
        {
            var imgStream = LoadGifImage(cacheKey, imgBytes, out errorMessage);
            if (imgStream == null)
                return;
            SetGifSource(imgStream);
        }

        #endregion
    }


    /// <summary>
    /// 加载静态图像
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="imgBytes"></param>
    /// <param name="pixelWidth"></param>
    /// <param name="isCache"></param>
    /// <returns></returns> 
    private IImage? LoadStaticImage(string cacheKey, byte[]? imgBytes, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (imgBytes == null)
            return null;
        if (ImageCacheList.TryGetValue(cacheKey, out var image))
            return image;
        try
        {
            using var stream = new MemoryStream(imgBytes);
            var img = new Bitmap(stream);
            ImageCacheList.TryAdd(cacheKey, img);
            return img;
        }
        catch (Exception ex)
        {
            errorMessage = $"LoadStaticImage Error {ex.Message}";
        }

        return null;
    }

    /// <summary>
    /// 加载Gif动图
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="imgBytes"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private Stream? LoadGifImage(string cacheKey, byte[]? imgBytes, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (imgBytes == null)
            return null;
        if (GifStreamCacheList.TryGetValue(cacheKey, out var image))
            return image;
        try
        {
            var stream = new MemoryStream(imgBytes);
            GifStreamCacheList.TryAdd(cacheKey, stream);
            return stream;
        }
        catch (Exception ex)
        {
            errorMessage = $"LoadStaticImage Error {ex.Message}";
        }

        return null;
    }


    /// <summary>
    /// 从网络下载图片
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private byte[]? LoadBytesFromHttp(string url)
    {
        try
        {
            var httpFactory = GlobalContext.Instance.GetHttpFactory();
            using var client = httpFactory.CreateClient();
            return client.GetByteArrayAsync(url).Result;
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
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private byte[]? LoadBytesFromLocal(string path, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (!System.IO.File.Exists(path))
        {
            errorMessage = "File No Exists";
            return null;
        }

        try
        {
            return File.ReadAllBytes(path);
        }
        catch (Exception ex)
        {
            errorMessage = $"Load Local Error {ex.Message}";
            return null;
        }
    }

    /// <summary>
    /// 加载AvaloniaResource资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private byte[]? LoadBytesFromApplicationResource(string path, out string errorMessage)
    {
        errorMessage = string.Empty;
        try
        {
            using var streamInfo = AssetLoader.Open(new Uri(path));
            var bytes = new byte[streamInfo.Length];
            _ = streamInfo.Read(bytes, 0, bytes.Length);
            return bytes;
        }
        catch (Exception ex)
        {
            errorMessage = $"Load Resource Error {ex.Message}";
            return null;
        }
    }

    private void SetStaticSource(IImage? source)
    {
        Dispatcher.UIThread.InvokeAsync((Action)delegate
        {
            if (this._imageContainer == null)
                return;

            if (this._staticImage == null)
                this._staticImage = new Image()
                {
                    Source = source,
                    [!Image.UseLayoutRoundingProperty] = this[!AsyncImage.UseLayoutRoundingProperty],
                    [!Image.StretchProperty] = this[!AsyncImage.StretchProperty]
                };
            else
                this._staticImage.Source = source;

            if (this._imageContainer.Child == null ||
                this._currentImageFormat == ImageHelper.ImageFormat.Gif)
                this._imageContainer.Child = _staticImage;
        });
    }

    private void SetGifSource(Stream? stream)
    {
        Dispatcher.UIThread.Invoke((Action)delegate
        {
            if (this._imageContainer == null)
                return;
            if (this._currentImageFormat != ImageHelper.ImageFormat.Gif)
            {
                this._imageContainer.Child = null;
            }

            if (this._gifImage == null)
                this._gifImage = new GifImage()
                {
                    SourceStream = stream,
                    AutoStart = true,
                    StretchDirection = StretchDirection.Both,
                    [!GifImage.StretchProperty] = this[!AsyncImage.StretchProperty]
                };
            else
                this._gifImage.SourceStream = stream;

            if (this._imageContainer.Child == null || this._currentImageFormat != ImageHelper.ImageFormat.Gif)
                this._imageContainer.Child = this._gifImage;
        });
    }


    /// <summary>
    /// 清除缓存
    /// </summary>
    /// <param name="cacheKey">缓存Key,格式： CacheGroup_UrlSource</param>
    public static void ClearCache()
    {
        if (!ImageCacheList.IsEmpty)
        {
            foreach (var key in ImageCacheList.Keys)
            {
                var bitmap = ImageCacheList[key];
                bitmap.Dispose();
            }
        }

        if (!GifStreamCacheList.IsEmpty)
        {
            foreach (var key in GifStreamCacheList.Keys)
            {
                var stream = GifStreamCacheList[key];
                stream.Close();
                stream.Dispose();
            }
        }


        ImageCacheList.Clear();
    }
}
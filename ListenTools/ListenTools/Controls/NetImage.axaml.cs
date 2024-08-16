using System;
using System.IO;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ListenTools.Global;

namespace ListenTools.Controls;

public partial class NetImage : UserControl
{
    public NetImage()
    {
        InitializeComponent();
    }

    #region DependencyProperty

    public static readonly StyledProperty<string> UrlSourceProperty =
        AvaloniaProperty.Register<NetImage, string>(nameof(UrlSource));

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<NetImage, Stretch>(nameof(Avalonia.Media.Stretch),
            defaultValue: Stretch.Uniform);

    public string UrlSource
    {
        get => GetValue(UrlSourceProperty);
        set => SetValue(UrlSourceProperty, value);
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    #endregion

    static NetImage()
    {
        UrlSourceProperty.Changed.AddClassHandler<NetImage>(UrlSourceChanged);
    }

    private Image? _staticImage;

    protected override async void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _staticImage = e.NameScope.Find<Image>("PART_StaticImage");
        if (_staticImage == null)
            return;

        if (!string.IsNullOrEmpty(this.UrlSource))
            _staticImage.Source = await Helper.ImageHelper.GetImageData(UrlSource);
    }


    private static async void UrlSourceChanged(NetImage s, AvaloniaPropertyChangedEventArgs e)
    {
        if (s._staticImage == null)
            return;

        if (e.NewValue is string newUrl)
        {
            s._staticImage.Source = await Helper.ImageHelper.GetImageData(newUrl);
        }
    }
}
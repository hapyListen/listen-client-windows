using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ListenTools.Controls;

public class IconPathButton : Button
{

    public static readonly StyledProperty<Geometry> IconPathProperty;
    public static readonly StyledProperty<IBrush?> OverBrushProperty;
    public static readonly StyledProperty<IBrush?> PressBrushProperty;
    public static readonly StyledProperty<IBrush?> OverForegroundProperty;
    public static readonly StyledProperty<IBrush?> PressForegroundProperty;
    public static readonly StyledProperty<Stretch> StretchProperty;

    static IconPathButton()
    {
        IconPathProperty = AvaloniaProperty.Register<IconPathButton, Geometry>(nameof(IconPath));
        OverBrushProperty = AvaloniaProperty.Register<IconPathButton, IBrush?>(nameof(OverBrush));
        PressBrushProperty = AvaloniaProperty.Register<IconPathButton, IBrush?>(nameof(PressBrush));
        OverForegroundProperty = AvaloniaProperty.Register<IconPathButton, IBrush?>(nameof(OverForeground));
        PressForegroundProperty = AvaloniaProperty.Register<IconPathButton, IBrush?>(nameof(PressForeground));
        StretchProperty = AvaloniaProperty.Register<IconPathButton, Stretch>(nameof(Stretch));
    }

    /// <summary>
    /// 图标内容
    /// </summary>
    public Geometry IconPath
    {
        get => GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }

    public IBrush? OverBrush
    {
        get => GetValue(OverBrushProperty);
        set => SetValue(OverBrushProperty, value);
    }

    public IBrush? PressBrush
    {
        get => GetValue(PressBrushProperty);
        set => SetValue(PressBrushProperty, value);
    }

    public IBrush? OverForeground
    {
        get => GetValue(OverForegroundProperty);
        set => SetValue(OverForegroundProperty, value);
    }

    public IBrush? PressForeground
    {
        get => GetValue(PressForegroundProperty);
        set => SetValue(PressForegroundProperty, value);
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

 
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ListenTools.Controls;

public class IconPathButton : Button
{

    public static readonly StyledProperty<Geometry> IconPathProperty;
    public static readonly StyledProperty<Stretch> StretchProperty;

    static IconPathButton()
    {
        IconPathProperty = AvaloniaProperty.Register<IconPathButton, Geometry>(nameof(IconPath));
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

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

 
}
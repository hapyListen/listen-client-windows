using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ListenTools.Controls;

public sealed class IconButton
{
    public static readonly AttachedProperty<Geometry?> IconPathProperty =
        AvaloniaProperty.RegisterAttached<Button, Geometry?>("IconPath",typeof(IconButton));

    public static readonly AttachedProperty<Stretch> IconStretchProperty =
        AvaloniaProperty.RegisterAttached<Button, Stretch>("IconStretch", typeof(IconButton),
            defaultValue: Stretch.Uniform);

    public static Geometry? GetIconPath(Button btn)
    {
        return btn.GetValue(IconButton.IconPathProperty);
    }

    public static void SetIconPath(Button btn, Geometry? path)
    {
        btn.SetValue(IconPathProperty, path);
    }

    public static Stretch GetIconStretch(Button btn)
    {
        return btn.GetValue(IconStretchProperty);
    }

    public static void SetIconStretch(Button btn, Stretch value)
    {
        btn.SetValue(IconStretchProperty, value);
    }

    static IconButton()
    {
        IconPathProperty.Changed.AddClassHandler<Button>(OnIconPathChanged);
    }

    public static void OnIconPathChanged(Button source, AvaloniaPropertyChangedEventArgs args)
    {
    }
}
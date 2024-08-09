using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ListenTools.Controls.Extends;

public sealed class ElementExtend
{
    public static readonly AttachedProperty<IBrush?> PointOverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PointOverBackground", typeof(ElementExtend));

    public static readonly AttachedProperty<IBrush?> PointOverForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PointOverForeground", typeof(ElementExtend));

    public static readonly AttachedProperty<IBrush?> PressedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PressedBackground",typeof(ElementExtend));

    public static readonly AttachedProperty<IBrush?> PressedForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PressedForeground",typeof(ElementExtend));
    
    public static void SetPointOverBackground(Control element, IBrush? value)
    {
        element.SetValue(PointOverBackgroundProperty, value);
    }

    public static IBrush? GetPointOverBackground(Control element)
    {
        return element.GetValue(PointOverBackgroundProperty);
    }
    
    public static void SetPointOverForeground(Control element, IBrush? value)
    {
        element.SetValue(PointOverForegroundProperty, value);
    }

    public static IBrush? GetPointOverForeground(Control element)
    {
        return element.GetValue(PointOverForegroundProperty);
    }
    
    public static void SetPressedBackground(Control element, IBrush? value)
    {
        element.SetValue(PressedBackgroundProperty, value);
    }

    public static IBrush? GetPressedBackground(Control element)
    {
        return element.GetValue(PressedBackgroundProperty);
    }
    
    public static void SetPressedForeground(Control element, IBrush? value)
    {
        element.SetValue(PressedForegroundProperty, value);
    }

    public static IBrush? GetPressedForeground(Control element)
    {
        return element.GetValue(PressedForegroundProperty);
    }
}
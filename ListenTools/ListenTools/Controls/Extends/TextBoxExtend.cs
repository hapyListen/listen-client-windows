using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ListenTools.Controls.Extends;

public sealed class TextBoxExtend
{
    public static readonly AttachedProperty<IBrush?> WaterMarkBrushProperty =
        AvaloniaProperty.RegisterAttached<TextBox, IBrush?>("WaterMarkBrush", typeof(TextBoxExtend),defaultValue:new SolidColorBrush(Color.Parse("#9FA2A7")));

    public static IBrush? GetWaterMarkBrush(TextBlock element)
    {
        return element.GetValue(WaterMarkBrushProperty);
    }

    public static void SetWaterMarkBrush(TextBlock element, IBrush? brush)
    {
        element.SetValue(WaterMarkBrushProperty, brush);
    }
}
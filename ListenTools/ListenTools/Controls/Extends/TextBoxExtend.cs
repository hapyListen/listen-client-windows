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
    
    #region 文本框获取焦点后的边框大小

    public static readonly AttachedProperty<Thickness> FocusedBorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("FocusedBorderThickness",typeof(TextBoxExtend));

    public static void SetFocusedBorderThickness(Control element, Thickness thickness)
    {
        element.SetValue(FocusedBorderThicknessProperty, thickness);
    }
    
    public static Thickness GetFocusedBorderThickness(Control element)
    {
        return element.GetValue(FocusedBorderThicknessProperty);
    }

    #endregion
    
    #region 文本框获取焦点后的边框颜色

    public static readonly AttachedProperty<IBrush?> FocusedBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "FocusedBorderBrush",typeof(TextBoxExtend));
    
    public static void SetFocusedBorderBrush(Control element, IBrush? value)
    {
        element.SetValue(FocusedBorderBrushProperty, value);
    }

    public static IBrush? GetFocusedBorderBrush(Control element)
    {
        return element.GetValue(FocusedBorderBrushProperty);
    }

    #endregion
}
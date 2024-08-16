using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace ListenTools.Controls.Extends;

public sealed class ElementExtend
{
    #region 鼠标移入后的背景色

    public static readonly AttachedProperty<IBrush?> PointOverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PointOverBackground", typeof(ElementExtend));
    public static void SetPointOverBackground(Control element, IBrush? value)
    {
        element.SetValue(PointOverBackgroundProperty, value);
    }

    public static IBrush? GetPointOverBackground(Control element)
    {
        return element.GetValue(PointOverBackgroundProperty);
    }

    #endregion

    #region 鼠标移入后的前景色

    public static readonly AttachedProperty<IBrush?> PointOverForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PointOverForeground", typeof(ElementExtend));
    public static void SetPointOverForeground(Control element, IBrush? value)
    {
        element.SetValue(PointOverForegroundProperty, value);
    }

    public static IBrush? GetPointOverForeground(Control element)
    {
        return element.GetValue(PointOverForegroundProperty);
    }
    
    #endregion
  
    #region 鼠标按下后的背景色

    public static readonly AttachedProperty<IBrush?> PressedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PressedBackground",typeof(ElementExtend));
    
    public static void SetPressedBackground(Control element, IBrush? value)
    {
        element.SetValue(PressedBackgroundProperty, value);
    }

    public static IBrush? GetPressedBackground(Control element)
    {
        return element.GetValue(PressedBackgroundProperty);
    }

    #endregion

    #region 鼠标按下后的前景色

    public static readonly AttachedProperty<IBrush?> PressedForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PressedForeground",typeof(ElementExtend));
  
    public static void SetPressedForeground(Control element, IBrush? value)
    {
        element.SetValue(PressedForegroundProperty, value);
    }

    public static IBrush? GetPressedForeground(Control element)
    {
        return element.GetValue(PressedForegroundProperty);
    }

    #endregion

    #region 鼠标移入后的边框颜色

    public static readonly AttachedProperty<IBrush?> PointOverBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PointOverBorderBrush", typeof(ElementExtend));
    public static void SetPointOverBorderBrush(Control element, IBrush? value)
    {
        element.SetValue(PointOverBorderBrushProperty, value);
    }

    public static IBrush? GetPointOverBorderBrush(Control element)
    {
        return element.GetValue(PointOverBorderBrushProperty);
    }

    #endregion

    #region 鼠标按下后的边框颜色

    public static readonly AttachedProperty<IBrush?> PressedBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "PressedBorderBrush",typeof(ElementExtend));
    
    public static void SetPressedBorderBrush(Control element, IBrush? value)
    {
        element.SetValue(PressedBorderBrushProperty, value);
    }

    public static IBrush? GetPressedBorderBrush(Control element)
    {
        return element.GetValue(PressedBorderBrushProperty);
    }

    #endregion

    #region 鼠标移入后的边框宽度

    public static readonly AttachedProperty<Thickness> PointOverBorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("PointOverBorderThickness",typeof(ElementExtend));

    public static void SetPointOverBorderThickness(Control element, Thickness thickness)
    {
        element.SetValue(PointOverBorderThicknessProperty, thickness);
    }
    
    public static Thickness GetPointOverBorderThickness(Control element)
    {
        return element.GetValue(PointOverBorderThicknessProperty);
    }

    #endregion
    
    #region 鼠标按下后的边框宽度

    public static readonly AttachedProperty<Thickness> PressedBorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("PressedBorderThickness",typeof(ElementExtend));

    public static void SetPressedBorderThickness(Control element, Thickness thickness)
    {
        element.SetValue(PressedBorderThicknessProperty, thickness);
    }
    
    public static Thickness GetPressedBorderThickness(Control element)
    {
        return element.GetValue(PressedBorderThicknessProperty);
    }

    #endregion
}
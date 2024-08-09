using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ListenTools.Controls.Extends;

public sealed class ButtonExtend
{
    public static readonly AttachedProperty<Geometry?> IconPathProperty =
        AvaloniaProperty.RegisterAttached<Button, Geometry?>("IconPath",typeof(ButtonExtend));

    public static readonly AttachedProperty<Stretch> IconStretchProperty =
        AvaloniaProperty.RegisterAttached<Button, Stretch>("IconStretch", typeof(ButtonExtend),
            defaultValue: Stretch.Uniform);

    public static readonly AttachedProperty<Geometry?> CheckedIconPathProperty = 
        AvaloniaProperty.RegisterAttached<Button, Geometry?>("CheckedIconPath",typeof(ButtonExtend));

    public static readonly AttachedProperty<PlacementMode> IconPlacementProperty =
        AvaloniaProperty.RegisterAttached<Button, PlacementMode>("IconPlacement",typeof(ButtonExtend),defaultValue:PlacementMode.Left);
    
  
    public static Geometry? GetIconPath(Button btn)
    {
        return btn.GetValue(ButtonExtend.IconPathProperty);
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
    
    public static Geometry? GetCheckedIconPath(Button btn)
    {
        return btn.GetValue(CheckedIconPathProperty);
    }

    public static void SetCheckedIconPath(Button btn, Geometry? path)
    {
        btn.SetValue(CheckedIconPathProperty, path);
    }

    public static PlacementMode GetIconPlacement(Button btn)
    {
        return btn.GetValue(IconPlacementProperty);
    }

    public static void SetIconPlacement(Button btn, PlacementMode mode)
    {
        btn.SetValue(IconPlacementProperty, mode);
    }
}
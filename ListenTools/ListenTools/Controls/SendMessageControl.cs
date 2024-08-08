using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;

namespace ListenTools.Controls;

/// <summary>
/// 消息发送控件
/// </summary>
[TemplatePart("PART_TextPresenter", typeof (Button))]
[TemplatePart("PART_ScrollViewer", typeof (Button))]
public class SendMessageControl : UserControl
{
    public static readonly StyledProperty<bool> IsExpandProperty = AvaloniaProperty.Register<SendMessageControl, bool>(
        nameof(IsExpand));

    public static readonly StyledProperty<string> MessageProperty = AvaloniaProperty.Register<SendMessageControl, string>(
        nameof(Message));

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// 是否展开聊天框
    /// </summary>
    public bool IsExpand
    {
        get => GetValue(IsExpandProperty);
        set => SetValue(IsExpandProperty, value);
    }
}
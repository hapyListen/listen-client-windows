using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ListenTools.Controls;

public partial class MessageEditControl : UserControl
{
    public MessageEditControl()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<bool> IsExpandProperty = AvaloniaProperty.Register<MessageEditControl, bool>(
        nameof(IsExpand));

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<MessageEditControl, string>(
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

    private TextBox? _inputMessageBox;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _inputMessageBox = e.NameScope.Find<TextBox>("PART_MessageBox");
        if (_inputMessageBox == null)
            return;
        _inputMessageBox.AddHandler(KeyDownEvent, InputMessageBoxOnKeyDown, RoutingStrategies.Tunnel);
        // _inputMessageBox.KeyDown += InputMessageBoxOnKeyDown;
        _inputMessageBox.DetachedFromVisualTree += InputMessageBoxOnDetachedFromVisualTree;

        base.OnApplyTemplate(e);
    }

    /// <summary>
    /// 更换模板时注销绑定的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InputMessageBoxOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (_inputMessageBox == null)
            return;
        // _inputMessageBox.KeyDown -= InputMessageBoxOnKeyDown;
        _inputMessageBox.RemoveHandler(KeyDownEvent, InputMessageBoxOnKeyDown);
        _inputMessageBox.DetachedFromVisualTree -= InputMessageBoxOnDetachedFromVisualTree;
    }

    /// <summary>
    /// 输入文本框的键盘按下事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void InputMessageBoxOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e is { KeyModifiers: KeyModifiers.Control, Key: Key.Enter })
        {
            // this.Message += System.Environment.NewLine;
            // _inputMessageBox.SelectionStart = this.Message.Length - 1;
            // e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            e.Handled = true;
        }
    }
}
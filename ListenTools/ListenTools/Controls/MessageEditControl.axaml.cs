using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ListenTools.Models;

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
            nameof(Message),defaultValue:"");

    public static readonly StyledProperty<IList<AtUserModel>> AtUsersProperty =
        AvaloniaProperty.Register<MessageEditControl, IList<AtUserModel>>(nameof(AtUsers),defaultBindingMode: BindingMode.TwoWay);


    /// <summary>
    /// 聊天框内的消息
    /// </summary>
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

    /// <summary>
    /// 被@的用户信息
    /// </summary>
    public IList<AtUserModel> AtUsers
    {
        get => GetValue(AtUsersProperty);
        set => SetValue(AtUsersProperty, value);
    }

    private TextBox? _inputMessageBox;
    private EmojiChooseControl? _emojiChooseControl;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _inputMessageBox = e.NameScope.Find<TextBox>("PART_MessageBox");
        if (_inputMessageBox != null)
        {
            _inputMessageBox.AddHandler(KeyDownEvent, InputMessageBoxOnKeyDown, RoutingStrategies.Tunnel);
            _inputMessageBox.DetachedFromVisualTree += InputMessageBoxOnDetachedFromVisualTree;
        }

        _emojiChooseControl = e.NameScope.Find<EmojiChooseControl>("PART_EmojiChoose");
        if (_emojiChooseControl != null)
        {
            _emojiChooseControl.AddHandler(EmojiChooseControl.CheckEmojiEvent, EmojiCheckedEvent, RoutingStrategies.Bubble);
            _emojiChooseControl.DetachedFromVisualTree += EmojiChooseOnDetachedFromVisualTree;
        }

        base.OnApplyTemplate(e);
    }

    /// <summary>
    /// 模版改变时解绑表情回调事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EmojiChooseOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (_emojiChooseControl == null)
            return;
        _emojiChooseControl.RemoveHandler(EmojiChooseControl.CheckEmojiEvent, EmojiCheckedEvent);
        _emojiChooseControl.DetachedFromVisualTree -= EmojiChooseOnDetachedFromVisualTree;
    }

    /// <summary>
    /// 绑定表情选中回调事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void EmojiCheckedEvent(object? sender, EmojiCheckedEventArgs e)
    {
        if (this._inputMessageBox == null)
            return;
        var insertIndex = this._inputMessageBox.SelectionStart;
        var insertTxt = $"[{e.EmojiName}]";
        this.Message = this.Message.Insert(insertIndex, insertTxt);
        this._inputMessageBox.SelectionStart = insertIndex + insertTxt.Length;
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
    

    /// <summary>
    /// 从被At的用户列表内删除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveAtUserButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is AtUserModel model)
        {
            btn.Click -= RemoveAtUserButton_OnClick;
            this.AtUsers?.Remove(model);
        }
    }
}
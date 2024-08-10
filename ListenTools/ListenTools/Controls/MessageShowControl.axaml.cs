using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ListenTools.Models.BindingModels.MessageModels;

namespace ListenTools.Controls;

public partial class MessageShowControl : UserControl
{
    public MessageShowControl()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<IEnumerable<MessageBase>> MessagesProperty =
        AvaloniaProperty.Register<MessageShowControl, IEnumerable<MessageBase>>(nameof(Messages));


    public IEnumerable<MessageBase> Messages
    {
        get => GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }
}
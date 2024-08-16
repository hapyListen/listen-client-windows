using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ListenTools.Views;

public partial class ChatRoomEditDialog : Window
{
    public ChatRoomEditDialog()
    {
        InitializeComponent();
    }

    private void WindowCloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close(null);
    }
}
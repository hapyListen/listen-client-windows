using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ListenTools.Global;

namespace ListenTools.Views.ChatRoomPages;

public partial class ChatRoomView : UserControl
{
    public ChatRoomView()
    {
        InitializeComponent();
        
        DataContext = GlobalContext.Instance.CreateViewModel<ViewModels.ChatRoomViewModels.ChatRoomViewModel>();
    }
}
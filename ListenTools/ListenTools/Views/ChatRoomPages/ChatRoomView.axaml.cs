using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using ListenTools.Global;
using ListenTools.ViewModels.ChatRoomViewModels;

namespace ListenTools.Views.ChatRoomPages;

public partial class ChatRoomView : UserControl
{
    private readonly ChatRoomViewModel? _viewModel;

    public ChatRoomView()
    {
        InitializeComponent();
        _viewModel = GlobalContext.Instance.CreateViewModel<ViewModels.ChatRoomViewModels.ChatRoomViewModel>();
        DataContext = _viewModel;
        this.RegisterMessage();
    }


    private void RegisterMessage()
    {
        _viewModel.MessengerPub.Register<string, string>(this, ChatRoomViewModel.OpenRoomEditDialogMsg, (s, e) =>
        {
            Dispatcher.UIThread.Post(delegate
            {
                ChatRoomEditDialog dialog = new();
                dialog.ShowDialog(MainWindow.GlobalMainWindow);
            });
        });
    }

    private void UnRegisterMessage()
    {
        _viewModel.MessengerPub.UnregisterAll(this);
    }
}
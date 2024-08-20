using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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
                // ChatRoomEditDialog dialog = new();
                // dialog.ShowDialog(MainWindow.GlobalMainWindow);
                var rootGrid = this.FindControl<Grid>("PART_RoomParentGrid");
                var settingMarkPanel = this.FindControl<Panel>("SettingMarkPanel");
                var settingControl = this.FindControl<ChatRoomSettingView>("PART_RoomSettingControl");
                if (rootGrid == null)
                    return;
                if (settingMarkPanel == null)
                    return;
                if (settingControl == null)
                    return;
                var rootGridScale = rootGrid.RenderTransform as ScaleTransform;
                rootGridScale.ScaleX = 0.95;
                rootGridScale.ScaleY = 0.95;
                
                // rootGrid.RenderTransform = new ScaleTransform(0.95, 0.95);
                settingMarkPanel.IsVisible = true;
                // settingControl.RenderTransform = new TranslateTransform(0, 0);
                var settingControlTransform = settingControl.RenderTransform as TranslateTransform;
                settingControlTransform.X = 0;
            });
        });
    }

    private void UnRegisterMessage()
    {
        _viewModel.MessengerPub.UnregisterAll(this);
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ListenTools.Views.Pages;

public partial class RoomListView : UserControl
{
    public RoomListView()
    {
        InitializeComponent();

        this.DataContext = GlobalContext.Instance.CreateViewModel<ViewModels.RoomListViewModel>();
    }
}
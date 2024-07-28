using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ListenTools.Models;

namespace ListenTools.ViewModels;

public partial class MainWindowViewModel : Base.ViewModelBase
{

    #region Properties

    /// <summary>
    /// 主菜单
    /// </summary>
    public IList<Models.MainMenuItemBdm> MainMenuSource { get; } = new ObservableCollection<MainMenuItemBdm>();

    #endregion
    
    public MainWindowViewModel()
    {
        Media.Instance.TcpClientTest();
        
        MainMenuSource.Add(new ()
        {
            Title = "主页",
            Icon = App.Current.FindResource("HomeIcon") as Geometry
        });
        MainMenuSource.Add(new ()
        {
            Title = "音乐",
            Icon = App.Current.FindResource("MusicIcon") as Geometry
        });
        MainMenuSource.Add(new ()
        {
            Title = "聊天室",
            Icon = App.Current.FindResource("ChatRoomIcon") as Geometry
        });
        
        MainMenuSource.Add(new ()
        {
            Title = "直播间",
            Icon = App.Current.FindResource("LiveHomeIcon") as Geometry
        }); 
      
    }
    
}
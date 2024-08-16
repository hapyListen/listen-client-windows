using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using ListenTools.Models;
using ListenTools.Models.BindingModels;
using ListenTools.Models.BindingModels.RoomModels;
using ListenTools.Models.Enums;

namespace ListenTools.ViewModels;

public class RoomListViewModel : Base.ViewModelBase
{
    #region Properties

    public IList<RoomCardBdm> HotRoomList { get; } =
        new ObservableCollection<RoomCardBdm>();
    
    /// <summary>
    /// 主菜单
    /// </summary>
    public IList<MainMenuItemBdm> MainMenuSource { get; } = new ObservableCollection<MainMenuItemBdm>();

    #endregion

    public RoomListViewModel()
    {
        
        MainMenuSource.Add(new()
        {
            Title = "首页",
            Icon = App.Current.FindResource("HomeIcon") as Geometry
        });

        MainMenuSource.Add(new()
        {
            Title = "聊天室",
            Icon = App.Current.FindResource("ChatRoomIcon") as Geometry
        });

        MainMenuSource.Add(new()
        {
            Title = "一起听",
            Icon = App.Current.FindResource("TogetherListenIcon") as Geometry
        });

        MainMenuSource.Add(new()
        {
            Title = "放映室",
            Icon = App.Current.FindResource("PlayRoomIcon") as Geometry
        });

        MainMenuSource.Add(new()
        {
            Title = "直播间",
            Icon = App.Current.FindResource("LiveHomeIcon") as Geometry
        });
        
        this.HotRoomList.Add(new()
        {
            RoomName = "荒野乱斗",
            RoomManName = "huangye",
            RoomBgImagePath =
                "https://p1.music.126.net/5sQ0Dmz4_nJMYtD0UXBs6w==/109951168645408205.jpg",
            RoomManIcon = "https://p1.music.126.net/5sQ0Dmz4_nJMYtD0UXBs6w==/109951168645408205.jpg",
            OnlineCount = 1000,
            RoomType = RoomTypes.ChatRoom,
            RoomDirection = "荒野乱斗是芬兰游戏荒野厂商Supercell开发的实时竞技手游，是集时下热门的丛林游戏、“吃鸡”等多种玩法！自2018年海外上线后便迅速走红，今年6月9日国服正式上线。"
        });
        
        this.HotRoomList.Add(new()
        {
            RoomName = "皇室战争",
            RoomManName = "黄石",
            RoomBgImagePath =
                "https://inews.gtimg.com/om_bt/OGlQWfsaAoKkuCcMZ2o9IVEPqd-72DQy5EAN02XBHUwfYAA/641",
            RoomManIcon = "https://inews.gtimg.com/om_bt/OGlQWfsaAoKkuCcMZ2o9IVEPqd-72DQy5EAN02XBHUwfYAA/641",
            OnlineCount = 231,
            RoomType= RoomTypes.LiveRoom,
            RoomDirection = "在《部落冲突：皇室战争》中卡牌的升级模式是收集相同的卡牌来进行融合升级卡牌等级。而卡牌的来源主要是靠开游戏中获得的各类宝箱或商店购买。"
        });
        
        this.HotRoomList.Add(new()
        {
            RoomName = "迷你世界",
            RoomManName = "战争",
            RoomBgImagePath =
                "https://p1.music.126.net/5sQ0Dmz4_nJMYtD0UXBs6w==/109951168645408205.jpg",
            RoomManIcon = "https://img0.baidu.com/it/u=3481995166,3315416959&fm=253&fmt=auto&app=120&f=JPEG?w=663&h=500",
            OnlineCount = 1,
            RoomType= RoomTypes.ShowingRoom,
            RoomDirection = "高度自由的3D沙盒游戏，这里没有等级和规则限制，这里没有特定的玩法，只有破坏和创造的乐趣。"
        });
    }
    
   
} 
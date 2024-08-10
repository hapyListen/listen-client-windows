using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ListenTools.Models.BindingModels;

namespace ListenTools.ViewModels.ChatRoomViewModels;

public partial class ChatRoomViewModel : Base.ViewModelBase
{
    #region Properties

    /// <summary>
    /// 房主信息
    /// </summary>
    [ObservableProperty] private Models.BindingModels.UserInfoBdm? _roomManInfo;


    /// <summary>
    /// 房间内所有用户信息（不包含房主）
    /// </summary>
    public IList<Models.BindingModels.UserInfoBdm> UserList { get; } = new ObservableCollection<UserInfoBdm>();

    #endregion

    public ChatRoomViewModel()
    {
        _roomManInfo = new ()
        { 
            HeadImage = "https://img0.baidu.com/it/u=3920778814,1443331810&fm=253&fmt=auto&app=120&f=JPEG?w=500&h=500",
            UserName = "眉上风止",
            UserNameColor = "#FFA78B",
            Direction = "你来时冬至，但眉上风止",
            IsOnline = true
        };
    }
}
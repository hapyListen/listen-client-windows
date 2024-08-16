using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ListenTools.Models.BindingModels;

namespace ListenTools.ViewModels.ChatRoomViewModels;

public partial class ChatRoomViewModel : Base.ViewModelBase
{
    /// <summary>
    /// 打开房间设置弹框
    /// </summary>
    public const string OpenRoomEditDialogMsg = nameof(OpenRoomEditDialogMsg);

    #region Properties

    /// <summary>
    /// 房主信息
    /// </summary>
    [ObservableProperty] private UserInfoBdm? _roomManInfo;


    /// <summary>
    /// 房间内所有用户信息（不包含房主）
    /// </summary>
    public IList<UserInfoBdm> UserList { get; } = new ObservableCollection<UserInfoBdm>();

    #endregion

    /// <summary>
    /// 打开房间编辑弹框
    /// </summary>
    public ICommand OpenEditDialogCommand { get; }

    public ChatRoomViewModel()
    {
        OpenEditDialogCommand = new Lazy<RelayCommand>(() => new RelayCommand(() =>
        {
            Messenger.Send(string.Empty, OpenRoomEditDialogMsg);
        })).Value;
        
        _roomManInfo = new()
        {
            HeadImage = "https://img0.baidu.com/it/u=3920778814,1443331810&fm=253&fmt=auto&app=120&f=JPEG?w=500&h=500",
            UserName = "眉上风止",
            UserNameColor = "#FFA78B",
            Summary = "你来时冬至，但眉上风止",
            IsOnline = true
        };
    }


}
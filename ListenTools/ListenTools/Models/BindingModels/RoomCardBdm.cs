using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi.Interfaces;

namespace ListenTools.Models.BindingModels;

public partial class RoomCardBdm : ObservableObject
{
    /// <summary>
    /// 房间ID
    /// </summary>
    public uint RoomtId { get; set; }
    
    /// <summary>
    /// 房主ID
    /// </summary>
    public uint RoomManId { get; set; }
    
    /// <summary>
    /// 房间名称
    /// </summary>
    [ObservableProperty] private string _roomName;

    /// <summary>
    /// 房主名称
    /// </summary>
    [ObservableProperty] private string _roomManName;

    /// <summary>
    /// 房主头像
    /// </summary>
    [ObservableProperty] private string _roomManIcon;
    
    /// <summary>
    /// 房间简介
    /// </summary>
    [ObservableProperty] private string _roomDirection;

    /// <summary>
    /// 房间背景图片地址
    /// </summary>
    [ObservableProperty] private string _roomBgImagePath;
    
    /// <summary>
    /// 房间在线人数
    /// </summary>
    [ObservableProperty] private uint _onlineCount;

    /// <summary>
    /// 房间类型，文字说明
    /// </summary>
    [ObservableProperty] private string _roomType;
}
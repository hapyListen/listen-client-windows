using CommunityToolkit.Mvvm.ComponentModel;
using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.RoomModels;

public class RoomCardBdm : ObservableObject
{
    /// <summary>
    /// 房间ID
    /// </summary>
    public uint RoomtId { get; set; }

    /// <summary>
    /// 房主ID
    /// </summary>
    public uint RoomManId { get; set; }
    

    private string? _roomName;
    /// <summary>
    /// 房间名称
    /// </summary>
    public string? RoomName
    {
        get => _roomName;
        set => SetProperty(ref _roomName, value);
    }
   
    private string? _roomManName;
    /// <summary>
    /// 房主名称
    /// </summary>
    public string? RoomManName
    {
        get => _roomManName;
        set => SetProperty(ref _roomManName, value);
    }

    private string? _roomManIcon;
    /// <summary>
    /// 房主头像
    /// </summary>
    public string? RoomManIcon
    {
        get => _roomManIcon;
        set => SetProperty(ref _roomManIcon, value);
    }

    private string? _roomDirection;
    /// <summary>
    /// 房间简介
    /// </summary>
    public string? RoomDirection
    {
        get => _roomDirection;
        set => SetProperty(ref _roomDirection, value);
    }

    private string? _roomBgImagePath;
    /// <summary>
    /// 房间背景图片地址
    /// </summary>
    public string? RoomBgImagePath
    {
        get => _roomBgImagePath;
        set => SetProperty(ref _roomBgImagePath, value);
    }
   
    private uint _onlineCount;
    /// <summary>
    /// 房间在线人数
    /// </summary>
    public uint OnlineCount
    {
        get => _onlineCount;
        set => SetProperty(ref _onlineCount, value);
    }

    private string? _roomTypeStr;
    /// <summary>
    /// 房间类型，文字说明
    /// </summary>
    public string? RoomTypeStr
    {
        get => _roomTypeStr;
        private set => SetProperty(ref _roomTypeStr, value);
    }

    private Enums.RoomTypes _roomType;
    public Enums.RoomTypes RoomType
    {
        get => _roomType;
        set
        {
            _roomType = value;
            switch (value)
            {
               
                case RoomTypes.ListenTogetherRoom:
                    RoomTypeStr = "一起听";
                    break;
                case RoomTypes.ShowingRoom:
                    RoomTypeStr = "放映室";
                    break;
                case RoomTypes.LiveRoom:
                    RoomTypeStr = "直播间";
                    break;
                default:
                case RoomTypes.ChatRoom:
                    RoomTypeStr = "聊天室";
                    break;
            }
        }
    }
}
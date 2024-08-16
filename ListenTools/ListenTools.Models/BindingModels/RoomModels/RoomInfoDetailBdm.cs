using System.Collections.ObjectModel;
using System.Reflection.Emit;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ListenTools.Models.BindingModels.RoomModels;

public class RoomInfoDetailBdm : ObservableObject
{
    public uint RoomId { get; set; }

    private string? _roomName;

    /// <summary>
    /// 房间名称
    /// </summary>
    public string? RoomName
    {
        get => _roomName;
        set => SetProperty(ref _roomName, value);
    }

    private string? _roomIcon;

    /// <summary>
    /// 房间头像
    /// </summary>
    public string? RoomIcon
    {
        get => _roomIcon;
        set => SetProperty(ref _roomIcon, value);
    }

    private string? _roomBackground;
    /// <summary>
    /// 房间背景图片
    /// </summary>
    public string? RoomBackground
    {
        get => _roomBackground;
        set => SetProperty(ref _roomBackground, value);
    }

    private Enums.RoomTypes _roomType;

    /// <summary>
    /// 房间类型
    /// </summary>
    public Enums.RoomTypes RoomType
    {
        get => _roomType;
        set => SetProperty(ref _roomType, value);
    }

    private uint _maxPeople;

    /// <summary>
    /// 房间最大人数
    /// </summary>
    public uint MaxPeople
    {
        get => _maxPeople;
        set => SetProperty(ref _maxPeople, value);
    }

    private string? _roomPassword;

    /// <summary>
    /// 房间密码
    /// </summary>
    public string? RoomPassword
    {
        get => _roomPassword;
        set => SetProperty(ref _roomPassword, value);
    }

    private string? _roomSummary;

    /// <summary>
    /// 房间介绍
    /// </summary>
    public string? RoomSummary
    {
        get => _roomSummary;
        set => SetProperty(ref _roomSummary, value);
    }

    private bool _isPublic;

    /// <summary>
    /// 房间是否公开
    /// </summary>
    public bool IsPublic
    {
        get => _isPublic;
        set => SetProperty(ref _isPublic, value);
    }

    private uint _getMaxRecord;

    /// <summary>
    /// 新成员获取的最大聊天记录
    /// </summary>
    public uint GetMaxRecord
    {
        get => _getMaxRecord;
        set => SetProperty(ref _getMaxRecord, value);
    }

    #region 一起听和放映室属性

    private bool _enbaleVote;
    /// <summary>
    /// 是否开启投票切歌
    /// </summary>
    public bool EnableVote
    {
        get => _enbaleVote;
        set => SetProperty(ref _enbaleVote, value);
    }

    private int _voteProportion;
    /// <summary>
    /// 投票占比
    /// </summary>
    public int VoteProportion
    {
        get => _voteProportion;
        set => SetProperty(ref _voteProportion, value);
    }

    #endregion

    #region 直播间属性

    private string? _livePath;
    /// <summary>
    /// 直播地址
    /// </summary>
    public string? LivePath
    {
        get => _livePath;
        set => SetProperty(ref _livePath, value);
    }

    #endregion
    
    /// <summary>
    /// 房间标签
    /// </summary>
    public IList<DataModels.RoomModels.RoomLabelType> Labels { get; } =
        new ObservableCollection<DataModels.RoomModels.RoomLabelType>();
}
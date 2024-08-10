namespace ListenTools.Models.Enums;

public enum RoomTypes : byte
{
    /// <summary>
    /// 纯聊天室
    /// </summary>
    ChatRoom = 0,
    /// <summary>
    /// 一起听
    /// </summary>
    ListenTogetherRoom,
    /// <summary>
    /// 放映室
    /// </summary>
    ShowingRoom,
    /// <summary>
    /// 直播间
    /// </summary>
    LiveRoom,
}
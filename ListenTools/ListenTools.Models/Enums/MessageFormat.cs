namespace ListenTools.Models.Enums;

public enum MessageSender : byte
{
    /// <summary>
    /// 自己发送的消息
    /// </summary>
    Self,
    /// <summary>
    /// 其他人发送的消息
    /// </summary>
    OtherUser,
    /// <summary>
    /// 系统推送过来的消息
    /// </summary>
    System,
}

public enum MessageFormat : byte
{
    /// <summary>
    /// 通知消息（房主更改聊天室信息、管理员踢人、禁言、解禁等操作，属于强制通知消息）
    /// </summary>
    Notify = 0,
    /// <summary>
    /// 提示消息（与通知消息不同，通知消息为强制推送，无法取消或关闭，但是提示消息可以被用户屏蔽，例如用户进入聊天室、点歌切歌这些，都属于提示）
    /// </summary>
    Tips,
    /// <summary>
    /// 文本消息
    /// </summary>
    Text,
    /// <summary>
    /// 表情消息
    /// </summary>
    Emoji,
    /// <summary>
    /// 图片消息
    /// </summary>
    Image,
    /// <summary>
    /// 文件消息
    /// </summary>
    File,
    /// <summary>
    /// @ 消息
    /// </summary>
    At,
    /// <summary>
    /// 交互消息（拍一拍）
    /// </summary>
    Mutual,
    /// <summary>
    /// 混合消息
    /// </summary>
    Mixed,
}

/// <summary>
/// 聊天室通知消息类型
/// </summary>
public enum ChatRoomNotifyType : byte
{
    /// <summary>
    /// 聊天室信息改变
    /// </summary>
    ChatRoomInfoChangeNotify,
    /// <summary>
    /// 管理踢人
    /// </summary>
    FuckOutNotify,
    /// <summary>
    /// 聊天室禁言
    /// </summary>
    ChatRoomMuteNotify,
    /// <summary>
    /// 解除禁言
    /// </summary>
    ChatRoomUnMuteNotify,
    /// <summary>
    /// 聊天室关闭
    /// </summary>
    ChatRoomCloseNotify
}

/// <summary>
/// 聊天室提示消息类型
/// </summary>
public enum ChatRoomTipsType : byte
{
    /// <summary>
    /// 用户进入聊天室
    /// </summary>
    UserJoinInTips,
    
    /// <summary>
    /// 用户退出聊天室
    /// </summary>
    UerQuitTips,
    
    /// <summary>
    /// 点播歌曲提示
    /// </summary>
    RequestSongTips,
    
    /// <summary>
    /// 切歌提示
    /// </summary>
    SwitchSongTips,
}
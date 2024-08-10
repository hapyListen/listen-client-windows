using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

public class NotifyMessage : MessageBase,IMessage
{
    public NotifyMessage()
    {
        MessageFormat = MessageFormat.Notify;
    }
    
    /// <summary>
    /// 通知类型
    /// </summary>
    public ChatRoomNotifyType NotifyType { get; set; }
    
    public string? Notify { get; set; }
    
    public string GetPreview()
    {
        return "[聊天室通知]";
    }
}
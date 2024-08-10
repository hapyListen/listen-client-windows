using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

public class AtMessage : MessageBase,IMessage
{
    public AtMessage()
    {
        this.MessageFormat = MessageFormat.At;
    }
    
    /// <summary>
    /// 被At的用户ID
    /// </summary>
    public uint UserId { get; set; }
    
    /// <summary>
    /// 被At的用户名
    /// </summary>
    public string? UserName { get; set; }
    
    public string GetPreview()
    {
        return $"[@{UserName}]";
    }
}
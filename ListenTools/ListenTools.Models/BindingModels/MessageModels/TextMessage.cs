using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

/// <summary>
/// 纯文本消息
/// </summary>
public class TextMessage : MessageBase,IMessage
{
    public TextMessage()
    {
        MessageFormat = MessageFormat.Text;
    }
    
    public string? Message { get; set; }
    
    public string GetPreview()
    {
        return string.IsNullOrEmpty(Message) ? $"[空消息]" : Message;
    }
}
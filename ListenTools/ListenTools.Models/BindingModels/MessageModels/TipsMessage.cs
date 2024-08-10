using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

public class TipsMessage : MessageBase,IMessage
{
    public TipsMessage()
    {
        MessageFormat = MessageFormat.Tips;
    }
    public string? Tips { get; set; }
    
    public string? GetPreview()
    {
        return Tips;
    }
}
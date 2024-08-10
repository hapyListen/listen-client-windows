using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

/// <summary>
/// 图片消息
/// </summary>
public class ImageMessage : MessageBase,IMessage
{
    public ImageMessage()
    {
        MessageFormat = MessageFormat.Image;
    }
    
    public string? ImagePath { get; set; }
    
    public string GetPreview()
    {
        return "[图片]";
    }
}
using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

public class EmojiMessage : MessageBase,IMessage
{
    public EmojiMessage()
    {
        MessageFormat = MessageFormat.Emoji;
    }
    
    /// <summary>
    /// 表情包名称
    /// </summary>
    public string? EmojiName { get; set; }
    
    /// <summary>
    /// 表情包的显示文字
    /// </summary>
    public string? EmojiPreview { get; set; }
    
    public string GetPreview()
    {
        return $"[{EmojiPreview}]";
    }
}
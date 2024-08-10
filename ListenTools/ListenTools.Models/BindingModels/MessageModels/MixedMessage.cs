using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

public class MixedMessage : MessageBase,IMessage
{
    public MixedMessage()
    {
        MessageFormat = MessageFormat.Mixed;
    }
    
    /// <summary>
    /// 表情包集合
    /// </summary>
    public IList<EmojiMessage>? EmojiMessages { get; set; }
    
    /// <summary>
    /// 图片集合
    /// </summary>
    public IList<ImageMessage>? ImageMessages { get; set; }
    
    /// <summary>
    /// At消息集合
    /// </summary>
    public IList<AtMessage>? AtMessages { get; set; }
    
    /// <summary>
    /// 文本消息
    /// </summary>
    public TextMessage? TextMessage { get; set; }
    
    public string GetPreview()
    {
        return "[组合消息]";
    }
}
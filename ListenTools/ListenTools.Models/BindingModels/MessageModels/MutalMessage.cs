using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

public class MutalMessage : MessageBase,IMessage
{
    public MutalMessage()
    {
        MessageFormat = MessageFormat.Mutual;
    }
    
    public uint TargetUserId { get; set; }
    
    public string? TargetUserName { get; set; }
    
    public string? GetPreview()
    {
        return $"\"{SenderName}\" 拍了拍 {TargetUserName}";
    }
}
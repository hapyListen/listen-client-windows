using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

public class FileMessage : MessageBase,IMessage
{
    public FileMessage()
    {
        MessageFormat = MessageFormat.File;
    }
    
    public string? FileName { get; set; }
    
    public string? FilePath { get; set; }
    
    public string GetPreview()
    {
        return "[文件]";
    }
}
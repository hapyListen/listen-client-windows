using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using ListenTools.Models.BindingModels.MessageModels;
using ListenTools.Models.Enums;

namespace ListenTools.Resources.DataTemplates.Selector;

/// <summary>
/// 消息聊天框中的位置
/// 自己发送的消息在右侧
/// 其他人发送的消息在左侧
/// 其他消息（通知、提示）等在中间
/// </summary>
public class ChatMessageTemplateSelector : IDataTemplate
{
    /// <summary>
    /// 自己发送的消息模板
    /// </summary>
    public IDataTemplate? SelfMessageTemplate { get; set; }
    /// <summary>
    /// 其他人发送的消息模板
    /// </summary>
    public IDataTemplate? OtherUserMessageTemplate { get; set; }
    /// <summary>
    /// 其他非用户推送的消息模板
    /// </summary>
    public IDataTemplate? SystemMessageTemplate { get; set; }
    
    public Control? Build(object? param)
    {
        if (param is not MessageBase messageBase)
            return null;

        return messageBase.SendFrom switch
        {
            MessageSender.Self => SelfMessageTemplate?.Build(param),
            MessageSender.OtherUser => OtherUserMessageTemplate?.Build(param),
            MessageSender.System => SystemMessageTemplate?.Build(param),
            _ => null
        };
    }

    public bool Match(object? data)
    {
        if (data is not MessageBase messageBase)
            return false;
        return messageBase.SendFrom is MessageSender.Self or MessageSender.OtherUser or MessageSender.System;
    }
}
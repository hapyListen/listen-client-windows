using System;
using Tmds.DBus.Protocol;

namespace ListenTools.Models.BindingModels.MessageModels;

public abstract class MessageBase : IMessage
{
    /// <summary>
    /// 消息ID (群聊的消息ID由客户端生成，生成规则为：房间ID+用户ID+时间戳+随机生成四位数)
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// 消息发送方ID
    /// </summary>
    public uint MessageSender { get; set; }

    /// <summary>
    /// 消息发送时间
    /// </summary>
    public DateTime? MessageSendTime { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageType MessageType { get; set; }

    /// <summary>
    /// 是否自己发送的消息
    /// </summary>
    public bool IsSelf { get; set; }

    public string GetPreview()
    {
        return $"[新消息]";
    }
}
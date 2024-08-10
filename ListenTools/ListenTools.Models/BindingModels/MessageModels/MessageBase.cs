using CommunityToolkit.Mvvm.ComponentModel;
using ListenTools.Models.Enums;

namespace ListenTools.Models.BindingModels.MessageModels;

public class MessageBase 
{
    /// <summary>
    /// 消息ID (群聊的消息ID由客户端生成，生成规则为：房间ID+用户ID+时间戳+随机生成四位数)
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// 消息发送方ID
    /// </summary>
    public uint SenderId { get; set; }

    /// <summary>
    /// 消息发送方名称
    /// </summary>
    public string? SenderName { get; set; }


    /// <summary>
    /// 消息发送方头像
    /// </summary>
    public string? SenderHeadImage { get; set; }

    /// <summary>
    /// 消息发送时间
    /// </summary>
    public DateTime? MessageSendTime { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public MessageFormat MessageFormat { get; protected set; }

    /// <summary>
    /// 消息发送对象
    /// </summary>
    public MessageSender SendFrom { get; set; }
}
namespace ListenTools.Media.Net;

/// <summary>
/// 消息处理
/// </summary>
public delegate void MessageProcessHandler(ArraySegment<byte> buffer, int readlength);
public sealed class MessageReceivedEventArgs
{
    private readonly MessageProcessHandler _processCallback = null;

    public readonly ArraySegment<byte> Buffer;

    public MessageReceivedEventArgs(ArraySegment<byte> buffer,MessageProcessHandler processCallback)
    {
        if (processCallback == null) throw new ArgumentException("processCallback");
        this.Buffer = buffer;
        this._processCallback = processCallback;
    }

    /// <summary>
    /// 设置读取长度
    /// </summary>
    /// <param name="readLength"></param>
    public void SetReadlength(int readLength)
    {
        this._processCallback(this.Buffer, readLength);
    }
}
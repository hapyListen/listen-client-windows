namespace ListenTools.Media.Net;

internal class Packet
{
    #region Members

    internal int SentSize = 0;

    public readonly byte[] Payload;

    #endregion

    public Packet(byte[] payload)
    {
        if (payload == null) throw new ArgumentException("payload");
        this.Payload = payload;
    }
        
    /// <summary>
    /// get or set tag object
    /// </summary>
    public object Tag { get; set; }
        
    /// <summary>
    /// is sent finish
    /// </summary>
    /// <returns></returns>
    public bool IsSent()
    {
        return this.SentSize == this.Payload.Length;
    }
    
}
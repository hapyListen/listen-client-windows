namespace ListenTools.Media.Net;

internal class PacketQueue
{
    #region Private Members

                /// <summary>
                /// 空闲中
                /// </summary>
                public const int IDLE = 1;
                /// <summary>
                /// 发送中
                /// </summary>
                public const int SENDING = 2;
                /// <summary>
                /// 入列中
                /// </summary>
                public const int ENQUEUE = 3;
                /// <summary>
                /// 出列中
                /// </summary>
                public const int DEQUEUE = 4;
                /// <summary>
                /// 已关闭
                /// </summary>
                public const int CLOSED = 5;

                /// <summary>
                /// 当前状态
                /// </summary>
                private int _state = IDLE;

                private Queue<Packet> _queue = new Queue<Packet>();
                private Action<Packet> _sendAction = null;

                #endregion

                #region Constructor

                /// <summary>
                /// new 
                /// </summary>
                /// <param name="sendAction"></param>
                /// <exception cref="ArgumentException"></exception>
                public PacketQueue(Action<Packet> sendAction)
                {
                    if (sendAction == null) throw new ArgumentException("sendAction");
                    this._sendAction = sendAction;
                }

                #endregion

                #region Public Methods

                /// <summary>
                /// try send packet
                /// </summary>
                /// <param name="packet"></param>
                /// <returns></returns>
                public bool TrySend(Packet packet)
                {
                    var spin = true;
                    while (spin)
                    {
                        switch (this._state)
                        {
                            case IDLE:
                                if (Interlocked.CompareExchange(ref this._state, SENDING, IDLE) == IDLE)
                                    spin = false;
                                break;
                            case SENDING:
                                if (Interlocked.CompareExchange(ref this._state, ENQUEUE, SENDING) == SENDING)
                                {
                                    this._queue.Enqueue(packet);
                                    this._state = SENDING;
                                    return true;
                                }
                                break;
                            case ENQUEUE:
                            case DEQUEUE:
                                Thread.Yield();
                                break;
                            case CLOSED:
                                return false;
                        }
                    }

                    this._sendAction(packet);
                    return true;
                }

                /// <summary>
                /// close
                /// </summary>
                /// <returns></returns>
                public CloseResult Close()
                {
                    var spin = true;
                    int beforeState = -1;
                    while (spin)
                    {
                        switch (this._state)
                        {
                            case IDLE :
                                if (Interlocked.CompareExchange(ref this._state, CLOSED, IDLE) == IDLE)
                                {
                                    spin = false;
                                    beforeState = IDLE;
                                }
                                break;
                            case SENDING:
                                if (Interlocked.CompareExchange(ref this._state, CLOSED, SENDING) == SENDING)
                                {
                                    spin = false;
                                    beforeState = SENDING;
                                }
                                break;
                            case ENQUEUE:
                            case DEQUEUE:
                                Thread.Yield();
                                break;
                            case CLOSED:
                                return new CloseResult(CLOSED, null);
                        }
                    }

                    var arrPackets = this._queue.ToArray();
                    this._queue.Clear();
                    this._queue = null;
                    this._sendAction = null;
                    return new CloseResult(beforeState, arrPackets);
                }
                
                /// <summary>
                /// try send next packet
                /// </summary>
                /// <returns></returns>
                public bool TrySendNext()
                {
                    var spin = true;
                    Packet packet = null;
                    while (spin)
                    {
                        switch (this._state)
                        {
                           case SENDING:
                               if (Interlocked.CompareExchange(ref this._state, DEQUEUE, SENDING) == SENDING)
                               {
                                   if (this._queue.Count == 0)
                                   {
                                       this._state = IDLE;
                                       return true;
                                   }

                                   packet = this._queue.Dequeue();
                                   this._state = SENDING;
                                   spin = false;
                               }
                               break;
                           case ENQUEUE:
                               Thread.Yield();
                               break;
                           case CLOSED:
                               return false;
                        }
                    }

                    this._sendAction(packet);
                    return true;
                }
                
                #endregion

                #region CloseResult

                /// <summary>
                /// close queue result
                /// </summary>
                public sealed class CloseResult
                {
                    /// <summary>
                    /// before close state
                    /// </summary>
                    public readonly int BeforeState;

                    /// <summary>
                    /// wait sending packet array
                    /// </summary>
                    public readonly Packet[] Packets;

                    /// <summary>
                    /// new
                    /// </summary>
                    /// <param name="beforeState"></param>
                    /// <param name="paclets"></param>
                    public CloseResult(int beforeState, Packet[] packets)
                    {
                        this.BeforeState = beforeState;
                        this.Packets = packets;
                    }
                }

                #endregion
}
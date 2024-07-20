using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ListenTools.Media.Net;

internal class TcpClient
{
    public event EventHandler<bool> OnConnectionStatusChanged;
    private readonly string _serverIp;
    private readonly int _serverPort;
    private readonly SocketAsyncEventArgsPool _socketAsyncEventArgsPool;
    private readonly PacketQueue _packetQueue;
    private const int SendTimeout = 5000;
    private const int ReceiveTimeout = 5000;
    private const int ConnectTimeout = 8000;
    private const int SocketBufferSize = 10240;
    private Socket _clientSocket;
    private SocketAsyncEventArgs? _saeSend;
    private SocketAsyncEventArgs? _saeReceive;
    private int _active = 1;
    private MemoryStream _tsStream = null;
    private int _isReceiving = 0;

    public bool Active => Thread.VolatileRead(ref this._active) == 1;

    public TcpClient(string ip, int port)
    {
        _serverIp = ip;
        _serverPort = port;
        _socketAsyncEventArgsPool = new SocketAsyncEventArgsPool(SocketBufferSize);
        _packetQueue = new PacketQueue(this.SendPacketInternal);
    }

    /// <summary>
    /// 异步发送数据
    /// </summary>
    /// <param name="packet"></param>
    public void BeginSend(Packet packet)
    {
        this._packetQueue.TrySend(packet);
    }

    /// <summary>
    /// 异步接收数据
    /// </summary>
    public void BeginReceive()
    {
        if (Interlocked.CompareExchange(ref this._isReceiving, 1, 0) == 0)
            this.ReceiveInternal();
    }

    /// <summary>
    /// 异步断开连接
    /// </summary>
    /// <param name="ex"></param>
    public void BeginDisconnect(Exception ex = null)
    {
        if (Interlocked.CompareExchange(ref this._active, 0, 1) == 1)
            this.DisconnectInternal(ex);
    }

    #region Connection

    public void BeginConnect()
    {
        var source = new TaskCompletionSource<Socket>();
        _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var e = new SocketAsyncEventArgs();
        e.UserToken = new Tuple<TaskCompletionSource<Socket>, Socket>(source, _clientSocket);
        e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(_serverIp), _serverPort);
        e.Completed += OnConnectCompleted;

        bool completed = true;
        try
        {
            completed = _clientSocket.ConnectAsync(e);
        }
        catch (Exception ex)
        {
            source.TrySetException(ex);
        }

        if (!completed)
            ThreadPool.QueueUserWorkItem(_ => OnConnectCompleted(null, e));

        source.Task.ContinueWith(ConnectionCallback);
    }

    /// <summary>
    /// connect completed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnConnectCompleted(object? sender, SocketAsyncEventArgs e)
    {
        var t = e.UserToken as Tuple<TaskCompletionSource<Socket>, Socket>;
        var source = t.Item1;
        var socket = t.Item2;
        var error = e.SocketError;

        e.UserToken = null;
        e.Completed -= OnConnectCompleted;
        e.Dispose();

        if (error != SocketError.Success)
        {
            socket.Close();
            source?.TrySetException(new SocketException((int)error));
            return;
        }

        source?.TrySetResult(socket);
    }

    private void ConnectionCallback(Task<Socket> t)
    {
        if (t.IsFaulted)
            return;

        var socket = t.Result;
        socket.NoDelay = true;
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

        socket.ReceiveBufferSize = SocketBufferSize;
        socket.SendBufferSize = SocketBufferSize;

        socket.SendTimeout = SendTimeout;
        socket.ReceiveTimeout = ReceiveTimeout;

        //init send
        this._saeSend = _socketAsyncEventArgsPool.Acquire();
        this._saeSend.Completed += this.SendAsyncCompleted;

        //init receive
        this._saeReceive = _socketAsyncEventArgsPool.Acquire();
        this._saeReceive.Completed += this.ReceiveAsyncCompleted;

        OnConnectionStatusChanged?.Invoke(this, true);
    }

    #endregion

    #region Disconnect

    /// <summary>
    /// disconnect
    /// </summary>
    /// <param name="reason"></param>
    private void DisconnectInternal(Exception reason)
    {
        var e = new SocketAsyncEventArgs();
        e.Completed += this.DisconnectAsyncCompleted;
        e.UserToken = reason;

        var completedAsync = true;
        try
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            completedAsync = _clientSocket.DisconnectAsync(e);
        }
        catch (Exception ex)
        {
            ThreadPool.QueueUserWorkItem(_ => this.DisconnectAsyncCompleted(this, e));
            return;
        }

        if (!completedAsync)
            ThreadPool.QueueUserWorkItem(_ => this.DisconnectAsyncCompleted(this, e));
    }

    /// <summary>
    /// async disconnect callback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisconnectAsyncCompleted(object sender, SocketAsyncEventArgs e)
    {
        try
        {
            _clientSocket.Close();
        }
        catch (Exception ex)
        {
        }

        var reason = e.UserToken as Exception;
        e.Completed -= this.DisconnectAsyncCompleted;
        e.Dispose();

        this.OnConnectionStatusChanged?.Invoke(this, false);
        Utils.TaskEx.Delay(new Random().Next(100, 1000)).ContinueWith(_ => this.BeginConnect());
        this.FreeSendQueue();
    }

    #endregion

    #region Send

    private Packet _currSendingPacket;

    /// <summary>
    /// internal send packet.
    /// </summary>
    /// <param name="packet"></param>
    /// <exception cref="ArgumentNullException">packet is null</exception>
    private void SendPacketInternal(Packet packet)
    {
        this._currSendingPacket = packet;
        this.SendPacketInternal(this._saeSend);
    }

    private void SendPacketInternal(SocketAsyncEventArgs e)
    {
        var packet = this._currSendingPacket;

        var length = Math.Min(packet.Payload.Length - packet.SentSize, SocketBufferSize);
        var completedAsync = true;
        try
        {
            Buffer.BlockCopy(packet.Payload, packet.SentSize, e.Buffer, 0, length);
            e.SetBuffer(0, length);
            completedAsync = _clientSocket.SendAsync(e);
        }
        catch (Exception ex)
        {
            this.BeginDisconnect(ex);
            this.FreeSend();
        }

        if (!completedAsync)
            ThreadPool.QueueUserWorkItem(_ => this.SendAsyncCompleted(this, e));
    }

    /// <summary>
    /// async send callback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SendAsyncCompleted(object sender, SocketAsyncEventArgs e)
    {
        var packet = this._currSendingPacket;

        if (e.SocketError != SocketError.Success)
        {
            this.BeginDisconnect(new SocketException((int)e.SocketError));
            this.FreeSend();
            return;
        }

        packet.SentSize += e.BytesTransferred;

        if (e.Offset + e.BytesTransferred < e.Count)
        {
            var completedAsync = true;
            try
            {
                e.SetBuffer(e.Offset + e.BytesTransferred, e.Count - e.BytesTransferred - e.Offset);
                completedAsync = _clientSocket.SendAsync(e);
            }
            catch (Exception ex)
            {
                this.BeginDisconnect(ex);
                this.FreeSend();
            }

            if (!completedAsync)
                ThreadPool.QueueUserWorkItem(_ => this.SendAsyncCompleted(sender, e));
        }
        else
        {
            if (packet.IsSent())
            {
                this._currSendingPacket = null;
                if (!this._packetQueue.TrySendNext()) this.FreeSend();
            }
            else this.SendPacketInternal(e);
        }
    }

    #endregion

    #region Receive

    private int ccc = 0;

    private void ReceiveInternal()
    {
        Debug.WriteLine($"开始接收:{++ccc}");
        bool completed = true;
        try
        {
            completed = _clientSocket.ReceiveAsync(this._saeReceive);
        }
        catch (Exception ex)
        {
            this.BeginDisconnect(ex);
            this.FreeReceive();
        }

        if (!completed)
            ThreadPool.QueueUserWorkItem(_ => this.ReceiveAsyncCompleted(this, this._saeReceive));
    }

    private void ReceiveAsyncCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError != SocketError.Success)
        {
            this.BeginDisconnect(new SocketException((int)e.SocketError));
            this.FreeReceive();
            return;
        }

        if (e.BytesTransferred < 1)
        {
            this.BeginDisconnect();
            this.FreeReceive();
            return;
        }

        ArraySegment<byte> buffer;
        var ts = this._tsStream;
        if (ts == null || ts.Length == 0)
            buffer = new ArraySegment<byte>(e.Buffer, 0, e.BytesTransferred);
        else
        {
            ts.Write(e.Buffer, 0, e.BytesTransferred);
            buffer = new ArraySegment<byte>(ts.GetBuffer(), 0, (int)ts.Length);
        }

        this.OnMessageReceived(new MessageReceivedEventArgs(buffer, this.MessageProcessCallback));
        Debug.WriteLine($"接收完毕：{--ccc}");
    }

    private void MessageProcessCallback(ArraySegment<byte> payload, int readlength)
    {
        if (readlength < 0 || readlength > payload.Count)
            throw new ArgumentOutOfRangeException("readlength",
                "readlength less than 0 or greater than payload.Count.");

        var ts = this._tsStream;
        if (readlength == 0)
        {
            if (ts == null) this._tsStream = ts = new MemoryStream(SocketBufferSize);
            else ts.SetLength(0);

            ts.Write(payload.Array, payload.Offset, payload.Count);
            this.ReceiveInternal();
            return;
        }

        if (readlength == payload.Count)
        {
            if (ts != null) ts.SetLength(0);
            this.ReceiveInternal();
            return;
        }

        //粘包处理
        this.OnMessageReceived(new MessageReceivedEventArgs(
            new ArraySegment<byte>(payload.Array, payload.Offset + readlength, payload.Count - readlength),
            this.MessageProcessCallback));
    }

    private void OnMessageReceived(MessageReceivedEventArgs e)
    {
        //process message
        int readlength = e.Buffer.Count;

        Debug.WriteLine($"接受消息长度：{readlength}");
        //continue receiveing..
        e.SetReadlength(readlength);
    }

    #endregion

    #region Free

    /// <summary>
    /// free send queue
    /// </summary>
    private void FreeSendQueue()
    {
        var result = this._packetQueue.Close();
        if (result.BeforeState == PacketQueue.CLOSED) return;
        if (result.BeforeState == PacketQueue.IDLE) this.FreeSend();
    }

    /// <summary>
    /// free for send.
    /// </summary>
    private void FreeSend()
    {
        this._currSendingPacket = null;
        this._saeSend.Completed -= this.SendAsyncCompleted;
        this._socketAsyncEventArgsPool.Release(this._saeSend);
        this._saeSend = null;
    }

    /// <summary>
    /// free fo receive.
    /// </summary>
    private void FreeReceive()
    {
        this._saeReceive.Completed -= this.ReceiveAsyncCompleted;
        this._socketAsyncEventArgsPool.Release(this._saeReceive);
        this._saeReceive = null;
        if (this._tsStream != null)
        {
            this._tsStream.Close();
            this._tsStream = null;
        }
    }

    #endregion
}
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ListenTools.Media.Net;

internal class TcpClient
{
    /// <summary>
    /// connection disconnected delegate
    /// </summary>
    public delegate void ConnectionStatusChangedHandler(Socket socket, bool status);

    public event ConnectionStatusChangedHandler OnConnectionStatusChanged;

    private readonly string _serverIp;
    private readonly int _serverPort;
    private readonly SocketAsyncEventArgsPool _socketAsyncEventArgsPool;
    private const int SendTimeout = 5000;
    private const int ReceiveTimeout = 5000;
    private const int ConnectTimeout = 8000;
    private const int SocketBufferSize = 10240;
    private Socket _clientSocket;
    private MemoryStream? _readStream;
    private SocketAsyncEventArgs? _saeReceive;
    private SocketAsyncEventArgs? _saeSend;

    public TcpClient(string ip, int port)
    {
        _serverIp = ip;
        _serverPort = port;
        _socketAsyncEventArgsPool = new SocketAsyncEventArgsPool(SocketBufferSize);
    }

    #region Connection

    public void Connect()
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
        {
            Utils.TaskEs.Delay(new Random().Next(500, 1500)).ContinueWith(_ => this.Connect());
            return;
        }

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

        OnConnectionStatusChanged?.Invoke(socket, true);
    }

    #endregion

    #region Disconnect

    /// <summary>
    /// disconnect
    /// </summary>
    private void Disconnect(Exception? err = null)
    {
        err ??= new SocketException((int)SocketError.Disconnecting);
        var e = new SocketAsyncEventArgs();
        e.Completed += this.DisconnectAsyncCompleted;

        var completedAsync = true;
        try
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            completedAsync = _clientSocket.DisconnectAsync(e);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"TcpSocket DisConnection : {err.Message}");
            Console.ResetColor();
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
    private void DisconnectAsyncCompleted(object? sender, SocketAsyncEventArgs e)
    {
        //dispose socket
        try
        {
            _clientSocket.Close();
        }
        catch (Exception ex)
        {
            // ignored
        }

        e.Completed -= this.DisconnectAsyncCompleted;
        e.Dispose();
        OnConnectionStatusChanged?.Invoke(_clientSocket, false);
    }

    #endregion

    #region Send

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data"></param>
    public void SendData(byte[] data)
    {
        SendData(new Packet(data));
    }

    /// <summary>
    /// internal send packet.
    /// </summary>
    /// <param name="e"></param>
    private void SendData(Packet packet)
    {
        var length = Math.Min(packet.Payload.Length - packet.SentSize, SocketBufferSize);
        var completedAsync = true;
        try
        {
            Buffer.BlockCopy(packet.Payload, packet.SentSize, _saeSend.Buffer, 0, length);
            _saeSend.SetBuffer(0, length);
            completedAsync = _clientSocket.SendAsync(_saeSend);
        }
        catch (Exception? ex)
        {
            this.Disconnect(ex);
            this.FreeSend();
        }

        if (!completedAsync)
            ThreadPool.QueueUserWorkItem(_ => this.SendAsyncCompleted(this, _saeSend));
    }

    /// <summary>
    /// async send callback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SendAsyncCompleted(object? sender, SocketAsyncEventArgs? e)
    {
        var packet = this._currSendingPacket;
        if (e.SocketError != SocketError.Success)
        {
            this.Disconnect(new SocketException((int)e.SocketError));
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
                completedAsync = this._socket.SendAsync(e);
            }
            catch (Exception ex)
            {
                this.BeginDisconnect(ex);
                this.FreeSend();
                this.OnSendCallback(packet, false);
                this.OnError(ex);
            }

            if (!completedAsync)
                ThreadPool.QueueUserWorkItem(_ => this.SendAsyncCompleted(sender, e));
        }
        else
        {
            if (packet.IsSent())
            {
                this._currSendingPacket = null;
                this.OnSendCallback(packet, true);
                // try send next packet
                if (!this._packetQueue.TrySendNext()) this.FreeSend();
            }
            else this.SendPacketInternal(e); //continue send this packet
        }
    }

    /// <summary>
    /// free for send
    /// </summary>
    private void FreeSend()
    {
        this._saeSend.Completed -= this.SendAsyncCompleted;
        _socketAsyncEventArgsPool.Release(this._saeSend);
        this._saeSend = null;
    }

    #endregion

    #region Receive

    /// <summary>
    /// receive
    /// </summary>
    private void ReceiveInternal()
    {
        bool completed = true;
        try
        {
            var socketAsyncEventArgs = this._saeReceive;
            if (socketAsyncEventArgs != null) completed = _clientSocket.ReceiveAsync(socketAsyncEventArgs);
        }
        catch (Exception ex)
        {
            this.Disconnect(ex);
            this.FreeReceive();
        }

        if (!completed)
            ThreadPool.QueueUserWorkItem(_ => this.ReceiveAsyncCompleted(this, this._saeReceive));
    }

    /// <summary>
    /// async receive callback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReceiveAsyncCompleted(object? sender, SocketAsyncEventArgs? e)
    {
        if (e.SocketError != SocketError.Success)
        {
            this.Disconnect(new SocketException((int)e.SocketError));
            this.FreeReceive();
            return;
        }

        if (e.BytesTransferred < 1)
        {
            this.Disconnect();
            this.FreeReceive();
            return;
        }

        ArraySegment<byte> buffer;
        var ts = this._readStream;
        if (ts == null || ts.Length == 0)
            buffer = new ArraySegment<byte>(e.Buffer, 0, e.BytesTransferred);
        else
        {
            ts.Write(e.Buffer, 0, e.BytesTransferred);
            buffer = new ArraySegment<byte>(ts.GetBuffer(), 0, (int)ts.Length);
        }

        this.OnMessageReceived(new MessageReceivedEventArgs(buffer, this.MessageProcessCallback));
    }

    /// <summary>
    /// message process callback
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="readlength"></param>
    private void MessageProcessCallback(ArraySegment<byte> payload, int readlength)
    {
        if (readlength < 0 || readlength > payload.Count)
            throw new ArgumentOutOfRangeException("readlength",
                "readlength less than 0 or greater than payload.Count.");
        var ts = this._readStream;
        if (readlength == 0)
        {
            if (ts == null) this._readStream = ts = new MemoryStream(SocketBufferSize);
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
    
    
    /// <summary>
    /// free to receive
    /// </summary>
    private void FreeReceive()
    {
        if (_saeReceive != null)
        {
            _saeReceive.Completed -= this.ReceiveAsyncCompleted;
            _socketAsyncEventArgsPool.Release(this._saeReceive);
        }

        _saeReceive = null;
        _readStream?.Close();
        _readStream?.Dispose();
        _readStream = null;
    }

    #endregion
}
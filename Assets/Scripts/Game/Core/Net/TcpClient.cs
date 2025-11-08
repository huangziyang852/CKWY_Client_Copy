using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Game.Core.Net
{
    /// <summary>
    ///     数据包类型
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        ///     包类型未被初始化
        /// </summary>
        None = 0,

        /// <summary>
        ///     连接服务器成功
        /// </summary>
        ConnectSuccess = 1,

        /// <summary>
        ///     连接服务器失败
        /// </summary>
        ConnectFailure = 2,

        /// <summary>
        ///     接收新的TCP数据包
        /// </summary>
        TcpPacket = 3,

        /// <summary>
        ///     断开链接
        /// </summary>
        ConnectDisconnect = 4
    }

    /// <summary>
    ///     网络包定义
    /// </summary>
    public class NetPacket
    {
        /// <summary>
        ///     包头占用8个字节，前4个时包体长度(不包含包头)，后4个是协议号
        /// </summary>
        public static int HEADER_SIZE = 8;

        /// <summary>
        ///     表示接收包头时接收到多少字节，接受包体时，代表包体收到多少字节
        /// </summary>
        public int currRecv;

        /// <summary>
        ///     包体数据 接收时调用
        /// </summary>
        public byte[] PacketBodyBytes;

        /// <summary>
        ///     包完整数据 发送时调用
        /// </summary>
        public byte[] PacketBytes = null;

        /// <summary>
        ///     包头数据 接收时调用
        /// </summary>
        public byte[] PacketHeaderBytes;

        /// <summary>
        ///     包类型
        /// </summary>
        public PacketType packetType = PacketType.None;

        /// <summary>
        ///     如果包类型是TcpPacket,则表示这个包的协议号，否则无效
        /// </summary>
        public int protoCode;

        public NetPacket(PacketType packetType)
        {
            this.packetType = packetType;
            protoCode = 0;
            currRecv = 0;
        }
    }


    /// <summary>
    ///     网络包队列,线程安全
    /// </summary>
    public class PacketQueue
    {
        private readonly Queue<NetPacket> netPackets = new();

        /// <summary>
        ///     网络包入队列
        /// </summary>
        /// <param name="netPacket"></param>
        public void Enqueue(NetPacket netPacket)
        {
            lock (netPackets)
            {
                netPackets.Enqueue(netPacket);
            }
        }

        /// <summary>
        ///     网络包出队列
        /// </summary>
        /// <returns></returns>
        public NetPacket Dequeue()
        {
            lock (netPackets)
            {
                if (netPackets.Count > 0) return netPackets.Dequeue();

                return null;
            }
        }

        /// <summary>
        ///     清空网络包队列
        /// </summary>
        public void Clear()
        {
            lock (netPackets)
            {
                netPackets.Clear();
            }
        }
    }

    /// <summary>
    ///     Tcp客户端类
    /// </summary>
    public class TcpClient:INetworkClient
    {
        /// <summary>
        ///     推送给主线程接收的网络包队列
        /// </summary>
        private readonly PacketQueue packetQueue = new();

        /// <summary>
        ///     这个TcpClient对象管理的客户端socket
        /// </summary>
        private Socket socket;

        /// <summary>
        ///     当前网络状态,false表示未连接
        /// </summary>
        private bool socketState;

        public bool IsConnected { get{ return socketState; }}

        /// <summary>
        ///     请求连接服务器，主线程调用
        /// </summary>
        /// <param name="address">服务器地址</param>
        /// <param name="port">端口号</param>
        public void Connect(string address, int port)
        {
            //保证线程同步，代码块不会被多个线程同时调用
            lock (this)
            {
                if (socketState == false)
                    try
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        Debug.Log("开始连接服务器" + address + ":" + port);
                        socket.BeginConnect(address, port, ConnectCallback, socket);
                    }
                    catch (Exception)
                    {
                        packetQueue.Enqueue(new NetPacket(PacketType.ConnectFailure));
                    }
            }
        }

        /// <summary>
        ///     请求链接服务器的回调
        /// </summary>
        /// <param name="asyncResult"></param>
        public void ConnectCallback(IAsyncResult asyncResult)
        {
            lock (this)
            {
                //多次链接发起时，只进行一次
                if (socketState) return;

                try
                {
                    //链接成功
                    socket = (Socket)asyncResult.AsyncState;

                    socketState = true;

                    socket.EndConnect(asyncResult);

                    packetQueue.Enqueue(new NetPacket(PacketType.ConnectSuccess));

                    //开始接收数据包包头
                    ReadPacket();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);

                    socket = null;

                    socketState = false;

                    packetQueue.Enqueue(new NetPacket(PacketType.ConnectFailure));
                }
            }
        }

        /// <summary>
        ///     主线程主动取走队列中所有的网络包(每帧调用)
        /// </summary>
        /// <returns></returns>
        public List<NetPacket> GetNetPackets()
        {
            var packetList = new List<NetPacket>();

            var one = packetQueue.Dequeue();
            while (one != null)
            {
                packetList.Add(one);
                one = packetQueue.Dequeue();
            }

            return packetList;
        }

        /// <summary>
        ///     发送网络包
        /// </summary>
        /// <param name="pCode"></param>
        /// <param name="nody"></param>
        public void SendAsync(int pCode, byte[] body)
        {
            var protoCode = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(pCode));
            var bodySize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(body.Length));
            var package = new byte[bodySize.Length + protoCode.Length + body.Length];
            Array.Copy(bodySize, 0, package, 0, bodySize.Length);
            Array.Copy(protoCode, 0, package, bodySize.Length, protoCode.Length);
            Array.Copy(body, 0, package, bodySize.Length + protoCode.Length, body.Length);

            SendAsync(package);
        }

        /// <summary>
        ///     主线程调用，发送网络字节流
        /// </summary>
        /// <param name="bytes"></param>
        public void SendAsync(byte[] bytes)
        {
            lock (this)
            {
                try
                {
                    if (socketState) socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendCallBack, socket);
                }
                catch (Exception e)
                {
                    Disconnect();
                }
            }
        }

        /// <summary>
        ///     发送的回调
        /// </summary>
        /// <param name="asyncResult"></param>
        public void SendCallBack(IAsyncResult asyncResult) //IAsyncResult是一个object，获取与当前操作关联的对象
        {
            lock (this)
            {
                try
                {
                    var socket = (Socket)asyncResult.AsyncState;

                    socket.EndSend(asyncResult);
                }
                catch (Exception e)
                {
                    Disconnect();
                }
            }
        }

        /// <summary>
        ///     读取数据包
        /// </summary>
        private void ReadPacket()
        {
            //创建一个Tcp的空包
            var netPacket = new NetPacket(PacketType.TcpPacket);

            //包头8个字节
            netPacket.PacketHeaderBytes = new byte[NetPacket.HEADER_SIZE];

            //开始接收远端发来的数据包头
            socket.BeginReceive(netPacket.PacketHeaderBytes, 0, NetPacket.HEADER_SIZE, SocketFlags.None, ReceiveHeader,
                netPacket);
        }

        /// <summary>
        ///     接收到数据包包头的回调函数
        /// </summary>
        /// <param name="asyncResult"></param>
        public void ReceiveHeader(IAsyncResult asyncResult)
        {
            lock (this)
            {
                try
                {
                    var netPacket = (NetPacket)asyncResult.AsyncState;

                    //实际读取到的字节数
                    var readSize = socket.EndReceive(asyncResult);

                    //服务器主动断开网络
                    if (readSize == 0)
                    {
                        Disconnect();

                        return;
                    }

                    netPacket.currRecv += readSize;

                    if (netPacket.currRecv == NetPacket.HEADER_SIZE)
                    {
                        //收到了约定的包头的长度，重置下标记，后面准备接收包体
                        netPacket.currRecv = 0;

                        //此包的包体大小(网络字节序转换主机字节序，可以无视大小端)
                        var bodySize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(netPacket.PacketHeaderBytes, 0));

                        //此包协议号
                        var protoCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(netPacket.PacketHeaderBytes, 4));

                        //会存在包体为0的情况
                        if (bodySize < 0)
                        {
                            Disconnect();
                            return;
                        }

                        //开始接收包体
                        netPacket.PacketBodyBytes = new byte[bodySize];

                        if (bodySize == 0)
                        {
                            packetQueue.Enqueue(netPacket);
                            //读取下一个包
                            ReadPacket();
                            return;
                        }

                        socket.BeginReceive(netPacket.PacketBodyBytes, 0, bodySize, SocketFlags.None, ReceiveBody,
                            netPacket);
                    }
                    else
                    {
                        //包头没收完，继续接收包头
                        var remainSize = NetPacket.HEADER_SIZE - netPacket.currRecv;
                        socket.BeginReceive(netPacket.PacketBodyBytes, netPacket.currRecv, remainSize, SocketFlags.None,
                            ReceiveHeader, netPacket);
                    }
                }
                catch (Exception e)
                {
                    Disconnect();
                }
            }
        }

        /// <summary>
        ///     接收包体后的回调
        /// </summary>
        /// <param name="asyncResult"></param>
        public void ReceiveBody(IAsyncResult asyncResult)
        {
            lock (this)
            {
                try
                {
                    var netPacket = (NetPacket)asyncResult.AsyncState;

                    //实际读取到的字节数
                    var readSize = socket.EndReceive(asyncResult);

                    //服务器主动断开网络
                    if (readSize == 0)
                    {
                        Disconnect();

                        return;
                    }

                    netPacket.currRecv += readSize;

                    if (netPacket.currRecv == netPacket.PacketBodyBytes.Length)
                    {
                        //收到了约定的包体的长度，重置下标记
                        netPacket.currRecv = 0;

                        packetQueue.Enqueue(netPacket);

                        //开始读取下一个包
                        ReadPacket();
                    }
                    else
                    {
                        //包没有收到足够长度的包体，继续接收包体
                        var remainSize = netPacket.PacketBodyBytes.Length - netPacket.currRecv;
                        socket.BeginReceive(netPacket.PacketBodyBytes, netPacket.currRecv, remainSize, SocketFlags.None,
                            ReceiveBody, netPacket);
                    }
                }
                catch (Exception e)
                {
                    Disconnect();
                }
            }
        }

        /// <summary>
        ///     断开网络链接，有可能主线程调用，也有可能IO线程调用
        /// </summary>
        public void Disconnect()
        {
            lock (this)
            {
                if (socketState)
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception e)
                    {
                        socket.Close();

                        socket = null;
                        socketState = false;
                        packetQueue.Clear();

                        packetQueue.Enqueue(new NetPacket(PacketType.ConnectDisconnect));
                    }
            }
        }
    }
}
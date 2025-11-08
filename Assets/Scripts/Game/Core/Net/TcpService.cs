using System;
using System.Collections.Generic;
using System.Net;
using Game.Core.Net.HttpProto;
using UnityEngine;

namespace Game.Core.Net
{
    public class TcpService : MonoBehaviour
    {
        public delegate void ConnectionFailedHandler();

        public delegate void ConnectionHandler();

        public delegate void DisconnectionHandler();

        // 定义委托类型
        public delegate void TcpPacketHandler(NetPacket data);

        private static TcpService _instance;
        private static GameObject _gameObject;


        private readonly Dictionary<int, IDispatcher> callbacks = new();
        private List<NetPacket> _netPackets;
        private INetworkClient _networkClient;


        public static TcpService Instance
        {
            get
            {
                if (_instance == null)
                {
                    if ((_instance = FindObjectOfType<TcpService>()) == null)
                    {
                        _gameObject = new GameObject(typeof(TcpService).ToString());
                        _instance = _gameObject.AddComponent<TcpService>();
                    }

                    if (Application.isPlaying) DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            //注册网络协议
            MsgHandler.RegisterHandlers();
            OnTcpPacket += OnReceivePacket;
        }

        // Update is called once per frame
        private void Update()
        {
            TCPUpdate();
        }

        // 定义事件
        public event TcpPacketHandler OnTcpPacket;
        public event ConnectionHandler OnConnectSuccess;
        public event ConnectionFailedHandler OnConnectFailed;
        public event DisconnectionHandler OnConnectDisconnect;

        public void TCPConnect(ServerInfo serverInfo)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                _networkClient = new WebSocketClient();
            }
            else
            {
                _networkClient = new TcpClient();
            }

            _networkClient.Connect(serverInfo.Ip, int.Parse(serverInfo.Port));
        }

        public void TCPUpdate()
        {
            // WebSocketClient 需要每帧调用 DispatchMessageQueue
            if (_networkClient is WebSocketClient wsClient)
            {
                wsClient.Update(); // 调用 DispatchMessageQueue
            }

            if (_networkClient != null) _netPackets = _networkClient.GetNetPackets();
            for (var i = 0; i < _netPackets.Count; i++)
            {
                var netPacket = _netPackets[i];
                switch (netPacket.packetType)
                {
                    case PacketType.TcpPacket:
                        OnTcpPacket?.Invoke(netPacket);
                        break;
                    case PacketType.ConnectSuccess:
                        OnConnectSuccess?.Invoke();
                        break;
                    case PacketType.ConnectFailure:
                        OnConnectFailed?.Invoke();
                        break;
                    case PacketType.ConnectDisconnect:
                        OnConnectDisconnect?.Invoke();
                        break;
                }
            }
        }

        public void SendAsync(int protoCode, byte[] body)
        {
            if (_networkClient != null) _networkClient.SendAsync(protoCode, body);
        }

        /// <summary>
        ///     发送有回调的消息
        /// </summary>
        /// <param name="protoCode">请求消息的code</param>
        /// <param name="receiveProtoCode">相应消息的code</param>
        /// <param name="body">消息体</param>
        /// <param name="callback">回调函数</param>
        public void SendMessageWithCallBack(int protoCode, int receiveProtoCode, byte[] body, Action<NetPacket> callback)
        {
            //先检查网络连接
            //TODO
            var cb = new Dispatcher();
            cb.processor += callback;
            if (!callbacks.ContainsKey(receiveProtoCode)) callbacks.Add(receiveProtoCode, cb);
            SendAsync(protoCode, body);
        }

        public void TCPDestory()
        {
            if (_networkClient != null) _networkClient.Disconnect();
        }

        public void OnReceivePacket(NetPacket data)
        {
            var protoCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data.PacketHeaderBytes, 4));
            if (callbacks.TryGetValue(protoCode, out var call))
            {
                if (!call.Process(data)) Debug.LogErrorFormat("Failed to process message. msgId : {0}", protoCode);
                callbacks.Remove(protoCode); // 在回调执行完成后再移除
            }
        }
    }
}
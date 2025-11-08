using NativeWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using UnityEngine;

namespace Game.Core.Net
{
    public class WebSocketClient:INetworkClient
    {
        private NativeWebSocket.WebSocket ws;
        private bool socketState;

        private readonly PacketQueue packetQueue = new(); // ���̶߳���
        private byte[] _recvBuffer = new byte[0];         // ����������

        public bool IsConnected => socketState;

        /// <summary>
        /// ���̵߳��ã�ȡ��Ϣ
        /// </summary>
        public List<NetPacket> GetNetPackets()
        {
            var packets = new List<NetPacket>();
            NetPacket one = packetQueue.Dequeue();
            while (one != null)
            {
                packets.Add(one);
                one = packetQueue.Dequeue();
            }
            return packets;
        }

        /// <summary>
        /// ���ӷ�����
        /// </summary>
        public async void Connect(string address, int port)
        {
            string url = $"";
            ws = new NativeWebSocket.WebSocket(url);

            ws.OnOpen += () =>
            {
                socketState = true;
                packetQueue.Enqueue(new NetPacket(PacketType.ConnectSuccess));
                Debug.Log("WebSocket connected!");
            };

            ws.OnMessage += (byte[] bytes) =>
            {
                Debug.Log("�յ���Ϣ:"+bytes.Length);
                ReceiveBytes(bytes);
            };

            ws.OnError += (e) =>
            {
                Debug.LogError("WebSocket error: " + e);
                packetQueue.Enqueue(new NetPacket(PacketType.ConnectFailure));
                socketState = false;
            };

            ws.OnClose += (e) =>
            {
                Debug.Log("WebSocket closed!");
                socketState = false;
                packetQueue.Enqueue(new NetPacket(PacketType.ConnectDisconnect));
            };

            try
            {
                await ws.Connect();
            }
            catch (Exception e)
            {
                Debug.LogError("WebSocket connect failed: " + e);
                packetQueue.Enqueue(new NetPacket(PacketType.ConnectFailure));
            }
        }

        /// <summary>
        /// ����Э��� + ����
        /// </summary>
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
        /// ����ԭʼ�ֽ�����
        /// </summary>
        public async void SendAsync(byte[] data)
        {
            if (!socketState || ws == null) return;

            try
            {
                await ws.Send(data);
            }
            catch (Exception e)
            {
                Debug.LogError("WebSocket send failed: " + e);
                Disconnect();
            }
        }

        /// <summary>
        /// �Ͽ�����
        /// </summary>
        public async void Disconnect()
        {
            socketState = false;
            if (ws != null)
            {
                await ws.Close();
                ws = null;
            }
            packetQueue.Clear();
            packetQueue.Enqueue(new NetPacket(PacketType.ConnectDisconnect));
        }

        /// <summary>
        /// ÿ֡Update���ã�WebSocket�������DispatchMessageQueue
        /// </summary>
        public void Update()
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                ws.DispatchMessageQueue();
            #endif
        }

        /// <summary>
        /// �����ֽڲ����
        /// </summary>
        private void ReceiveBytes(byte[] bytes)
        {
            Debug.Log($"_recvBuffer length: {_recvBuffer.Length}");
            string hex = BitConverter.ToString(bytes);
            Debug.Log($"Received {bytes.Length} bytes: {hex}");

            int oldLength = _recvBuffer.Length;
            Array.Resize(ref _recvBuffer, oldLength + bytes.Length);
            Array.Copy(bytes, 0, _recvBuffer, oldLength, bytes.Length);

            int offset = 0;
            while (_recvBuffer.Length - offset >= NetPacket.HEADER_SIZE)
            {
                // ��ȡ��ͷ��8�ֽڣ�
                int bodySize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_recvBuffer, offset));
                int protoCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_recvBuffer, offset + 4));

                Debug.Log($"offset={offset}, bufferLen={_recvBuffer.Length}, bodySize={bodySize}, protoCode={protoCode}");

                // ���������ʣ�೤�Ȳ���һ�����������͵���һ֡
                if (_recvBuffer.Length - offset - NetPacket.HEADER_SIZE < bodySize)
                    break;

                // ��ȡ����
                var bodyBytes = new byte[bodySize];
                Array.Copy(_recvBuffer, offset + NetPacket.HEADER_SIZE, bodyBytes, 0, bodySize);

                // ��װ NetPacket
                NetPacket packet = new NetPacket(PacketType.TcpPacket)
                {
                    PacketHeaderBytes = new byte[NetPacket.HEADER_SIZE],
                    PacketBodyBytes = bodyBytes,
                    protoCode = protoCode
                };
                Array.Copy(_recvBuffer, offset, packet.PacketHeaderBytes, 0, NetPacket.HEADER_SIZE);

                // ���
                packetQueue.Enqueue(packet);

                offset += NetPacket.HEADER_SIZE + bodySize;
            }

            // ��û������Ĳа����ڻ���
            int remain = _recvBuffer.Length - offset;
            var temp = new byte[remain];
            Array.Copy(_recvBuffer, offset, temp, 0, remain);
            _recvBuffer = temp;
        }
    }
}


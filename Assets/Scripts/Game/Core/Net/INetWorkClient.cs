using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Net
{
    public interface INetworkClient
    {
        void Connect(string address, int port);
        void SendAsync(int protoCode, byte[] body);
        List<NetPacket> GetNetPackets();
        void Disconnect();
        bool IsConnected { get; }
    }

}
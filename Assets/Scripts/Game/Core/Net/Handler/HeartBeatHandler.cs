using System.Threading.Tasks;
using Google.Protobuf;
using LaunchPB;
using UnityEngine;

namespace Game.Core.Net.Handler
{
    public class HeartBeatHandler : IMessageHandler
    {
        public Task<IMessage> Handle(IMessage message)
        {
            {
                if (message is not HeartBeat heartBeat)
                {
                    Debug.LogError("Handle ʧ��: ����� Message ���ʹ���");
                    return Task.FromResult<IMessage>(null);
                }

                TcpService.Instance.SendAsync((int)ProtoCode.EHeartBeat, heartBeat.ToByteArray());
                return Task.FromResult<IMessage>(null);
            }
        }
    }
}
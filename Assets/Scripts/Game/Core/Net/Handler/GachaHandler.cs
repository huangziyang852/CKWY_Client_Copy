using System.Threading.Tasks;
using Google.Protobuf;
using LaunchPB;
using UnityEngine;

namespace Game.Core.Net.Handler
{
    public class GachaHandler : IMessageHandler
    {
        public async Task<IMessage> Handle(IMessage message)
        {
            if (message is not GachaReq gachaReq)
            {
                Debug.LogError("Handle ʧ��: ����� Message ���ʹ���");
                return null;
            }

            var tcs = new TaskCompletionSource<GachaResultResp>(); // ���ڷ����첽���

            TcpService.Instance.SendMessageWithCallBack((int)ProtoCode.EGacha,
                (int)ProtoCode.EGachaResultResp, gachaReq.ToByteArray(), response =>
                {
                    IMessage gachaResultResp = new GachaResultResp();
                    var gachaResult =
                        gachaResultResp.Descriptor.Parser.ParseFrom(response.PacketBodyBytes, 0,
                            response.PacketBodyBytes.Length) as GachaResultResp;
                    tcs.SetResult(gachaResult);
                    Debug.Log("�յ���gachaResultResp:" + gachaResult);
                });
            return await tcs.Task;
        }
    }
}
using System.Threading.Tasks;
using Google.Protobuf;
using LaunchPB;
using UnityEngine;

namespace Game.Core.Net.Handler
{
    public class GetPlayerInfoHandler : IMessageHandler
    {
        public async Task<IMessage> Handle(IMessage message)
        {
            if (message is not GetPlayerInfo getPlayerInfo)
            {
                Debug.LogError("Handle 失败: 传入的 Message 类型错误");
                return null;
            }

            var tcs = new TaskCompletionSource<GetPlayerInfoResp>(); // 用于返回异步结果

            TcpService.Instance.SendMessageWithCallBack((int)ProtoCode.EGetPlayerInfo,
                (int)ProtoCode.EGetPlayerInfoResp, getPlayerInfo.ToByteArray(), response =>
                {
                    Debug.Log("收到了获取用户信息的消息" + response);
                    IMessage getPlayerInfoResp = new GetPlayerInfoResp();
                    var playerInfo =
                        getPlayerInfoResp.Descriptor.Parser.ParseFrom(response.PacketBodyBytes, 0,
                            response.PacketBodyBytes.Length) as GetPlayerInfoResp;
                    tcs.SetResult(playerInfo);
                });
            return await tcs.Task;
        }
    }
}
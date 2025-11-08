using System.Threading.Tasks;
using Google.Protobuf;
using LaunchPB;
using UnityEngine;

namespace Game.Core.Net.Handler
{
    public class GetWorldInfoHandler : IMessageHandler
    {
        public async Task<IMessage> Handle(IMessage message)
        {
            if (message is not GetWorldInfo getWorldInfo)
            {
                Debug.LogError("Handle 失败: 传入的 Message 类型错误");
                return null;
            }

            var tcs = new TaskCompletionSource<GetWorldInfoResp>(); // 用于返回异步结果

            TcpService.Instance.SendMessageWithCallBack((int)ProtoCode.EGetWorldInfo,
                (int)ProtoCode.EGetWorldInfoResp, getWorldInfo.ToByteArray(), response =>
                {
                    Debug.Log("收到了获取用户信息的消息" + response);
                    IMessage getWorldInfoResp = new GetWorldInfoResp();
                    var worldInfo =
                        getWorldInfoResp.Descriptor.Parser.ParseFrom(response.PacketBodyBytes, 0,
                            response.PacketBodyBytes.Length) as GetWorldInfoResp;
                    tcs.SetResult(worldInfo);
                });
            return await tcs.Task;
        }
    }
}
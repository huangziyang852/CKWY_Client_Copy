using System.Threading.Tasks;
using Google.Protobuf;
using LaunchPB;
using UnityEngine;

namespace Game.Core.Net.Handler
{
    public class GetHeroInfoHandler : IMessageHandler
    {
        public async Task<IMessage> Handle(IMessage message)
        {
            if (message is not GetHeroInfo getHeroInfo)
            {
                Debug.LogError("Handle 失败: 传入的 Message 类型错误");
                return null;
            }

            var tcs = new TaskCompletionSource<GetHeroInfoResp>(); // 用于返回异步结果

            TcpService.Instance.SendMessageWithCallBack((int)ProtoCode.EGetHeroInfo,
                (int)ProtoCode.EGetHeroInfoResp, getHeroInfo.ToByteArray(), response =>
                {
                    Debug.Log("收到了获取用户信息的消息" + response);
                    IMessage getHeroInfoResp = new GetHeroInfoResp();
                    var heroInfo =
                        getHeroInfoResp.Descriptor.Parser.ParseFrom(response.PacketBodyBytes, 0,
                            response.PacketBodyBytes.Length) as GetHeroInfoResp;
                    tcs.SetResult(heroInfo);
                });
            return await tcs.Task;
        }
    }
}
using Google.Protobuf;
using LaunchPB;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Core.Net.Handler
{
    public class GetItemInfoHandler : IMessageHandler
    {
        public async Task<IMessage> Handle(IMessage message)
        {
            if (message is not GetItemInfo getItemInfo)
            {
                Debug.LogError("Handle 失败: 传入的 Message 类型错误");
                return null;
            }

            var tcs = new TaskCompletionSource<GetItemInfoResp>(); // 用于返回异步结果

            TcpService.Instance.SendMessageWithCallBack((int)ProtoCode.EGetItemInfo,
                (int)ProtoCode.EGetItemInfoResp, getItemInfo.ToByteArray(), response =>
                {
                    Debug.Log("收到了获取用户道具信息的消息" + response);
                    IMessage getItemInfoResp = new GetItemInfoResp();
                    var itemInfo =
                        getItemInfoResp.Descriptor.Parser.ParseFrom(response.PacketBodyBytes, 0,
                            response.PacketBodyBytes.Length) as GetItemInfoResp;
                    tcs.SetResult(itemInfo);
                });
            return await tcs.Task;
        }
    }

}
using System;
using System.Collections.Generic;
using Game.OutGame.Controller;
using Google.Protobuf;
using LaunchPB;

namespace Game.Core.Net
{
    public class MsgHandler
    {
        public static Dictionary<int, Action<NetPacket>> proto2FunDic = new();

        public static void RegisterHandlers()
        {
            //添加Handler方法
            //proto2FunDic[(int)ProtoCode.ELoginResp] = LoginRes;

            //proto2FunDic[1] = GetPlayerInfoRes;
        }

        //public static void Proto2Func(NetPacket netPacket)
        //{
        //    Debug.Log("收到网络包" + netPacket.protoCode);
        //    if (proto2FunDic.TryGetValue(netPacket.protoCode, out var handler))
        //        handler(netPacket);
        //    else
        //        Debug.LogError($"未找到{netPacket.protoCode}对应的handler");
        //}

        private static async void LoginRes(NetPacket netPacket)
        {
            IMessage message = new LoginResp();

            var login =
                message.Descriptor.Parser.ParseFrom(netPacket.PacketBodyBytes, 0, netPacket.PacketBodyBytes.Length) as
                    LoginResp;

            await LoginMgr.Instance.OnLoginSuccess(login);
        }
    }
}

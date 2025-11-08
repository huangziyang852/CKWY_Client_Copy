using Game.Core.Net.Handler;
using LaunchPB;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Core.Net.Service
{
    public class ItemService : BaseService<ItemService>
    {
        public async Task<GetItemInfoResp> GetItemInfoAsync()
        {
            IMessageHandler handler = new GetItemInfoHandler();
            var respone = await handler.Handle(new GetItemInfo()) as GetItemInfoResp;
            if (respone == null) return null;
            return respone;
        }
    }
}

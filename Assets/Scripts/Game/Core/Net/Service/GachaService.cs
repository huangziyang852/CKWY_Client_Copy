using System.Threading.Tasks;
using Game.Core.Manager;
using Game.Core.Net.Handler;
using LaunchPB;

namespace Game.Core.Net.Service
{
    public class GachaService : BaseService<GachaService>
    {
        public async Task<GachaResultResp> GachaAsync(int gachaId, int gachaTimes)
        {
            IMessageHandler handler = new GachaHandler();
            var req = new GachaReq { GachaId = gachaId, GachaTimes = gachaTimes };
            var respone = await handler.Handle(req) as GachaResultResp;
            if (respone == null) return null;
            if(respone.AddItem!= null && respone.AddItem.Count>0)
            {
                foreach( var item in respone.AddItem )
                {
                    GameModelManager.Instance.ItemInfoModel.AddOrUpdateItem(item.ItemId,item.ItemCount);
                }
            }
            if (respone.AddHero != null && respone.AddHero.Count > 0)
            {
                foreach (var hero in respone.AddHero)
                {
                    GameModelManager.Instance.HeroInfoModel.AddOrUpdateHero(hero);
                }
            }
            EventBus<GachaResultEvent>.Publish(new GachaResultEvent(respone));
            return respone;
        }
    }
}
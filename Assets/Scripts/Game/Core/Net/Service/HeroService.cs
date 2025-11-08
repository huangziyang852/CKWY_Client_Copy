using System.Threading.Tasks;
using Game.Core.Net.Handler;
using LaunchPB;

namespace Game.Core.Net.Service
{
    public class HeroService : BaseService<HeroService>
    {
        public async Task<GetHeroInfoResp> GetHeroInfoAsync()
        {
            IMessageHandler handler = new GetHeroInfoHandler();
            var respone = await handler.Handle(new GetHeroInfo()) as GetHeroInfoResp;
            if (respone == null) return null;
            return respone;
        }
    }
}
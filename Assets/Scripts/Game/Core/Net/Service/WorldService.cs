using System.Threading.Tasks;
using Game.Core.Net.Handler;
using LaunchPB;

namespace Game.Core.Net.Service
{
    public class WorldService : BaseService<WorldService>
    {
        public async Task<GetWorldInfoResp> GetWorldInfoAsync()
        {
            IMessageHandler handler = new GetWorldInfoHandler();
            var respone = await handler.Handle(new GetWorldInfo()) as GetWorldInfoResp;
            if (respone == null) return null;
            return respone;
        }
    }
}
using System.Threading.Tasks;
using Game.Core.Net.Handler;
using LaunchPB;

namespace Game.Core.Net.Service
{
    public class UserService : BaseService<UserService>
    {
        public async Task<GetPlayerInfoResp> GetPlayerInfoAsync()
        {
            IMessageHandler handler = new GetPlayerInfoHandler();
            var respone = await handler.Handle(new GetPlayerInfo()) as GetPlayerInfoResp;
            if (respone == null) return null;
            return respone;
        }
    }
}
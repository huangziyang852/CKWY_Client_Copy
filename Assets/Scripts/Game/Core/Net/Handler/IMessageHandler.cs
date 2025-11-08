using System.Threading.Tasks;
using Google.Protobuf;

namespace Game.Core.Net.Handler
{
    public interface IMessageHandler
    {
        Task<IMessage> Handle(IMessage message);
    }
}
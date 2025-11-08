using System;
using System.Threading;
using Game.Core.Net.Handler;
using LaunchPB;

namespace Game.Core.Net.Service
{
    public class HeartBeatService : BaseService<HeartBeatService>
    {
        private Timer _timer;

        public void Start()
        {
            _timer = new Timer(SendHeartBeat, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private void SendHeartBeat(object state)
        {
            IMessageHandler handler = new HeartBeatHandler();
            handler.Handle(new HeartBeat());
            Console.WriteLine($"[{DateTime.Now}] 发送心跳包");
        }

        public void Stop()
        {
            _timer?.Dispose();
        }
    }
}
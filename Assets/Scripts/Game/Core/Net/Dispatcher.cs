using System;
using UnityEngine;

namespace Game.Core.Net
{
    /// <summary>
    ///     Message dispatcher interface.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        ///     Process message.
        /// </summary>
        /// <param name="data">Message data.</param>
        /// <return>Process result</return>
        public bool Process(NetPacket data);
    }


    public class Dispatcher : IDispatcher
    {
        public bool Process(NetPacket data)
        {
            try
            {
                if (data != null && processor != null) processor.Invoke(data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed to parse network message : {0}", e.Message);
                return false;
            }
        }

        public event Action<NetPacket> processor;
    }
}
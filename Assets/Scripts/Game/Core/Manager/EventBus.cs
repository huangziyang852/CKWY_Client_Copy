using LaunchPB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game.Common.Const.GameConst.ImagesPath.BG;

namespace Game.Core.Manager
{
    public static class EventBus<T>
    {
        private static readonly List<Action<T>> _listeners = new();

        public static void Subscribe(Action<T> listener)
        {
            if(!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public static void Unsubscribe(Action<T> listener)
        {
            _listeners.Remove(listener);
        }

        public static void Publish(T data)
        {
            foreach (var listener in _listeners.ToArray())
                listener?.Invoke(data);
        }
    }

    public class GachaRequestEvent
    {
        public int GachaId { get; }
        public int Count { get; }

        public GachaRequestEvent(int gachaId, int count)
        {
            GachaId = gachaId;
            Count = count;
        }
    }

    public class GachaResultEvent
    {
        public GachaResultResp GachaResult { get; }
        public GachaResultEvent(GachaResultResp gachaResult)
        {
            GachaResult = gachaResult;
        }
    }
}


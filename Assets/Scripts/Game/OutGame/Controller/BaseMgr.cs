using System;

namespace Game.OutGame.Controller
{
    public abstract class BaseMgr<T> where T : BaseMgr<T>, new()
    {
        private static readonly Lazy<T> _instance = new(() => new T());
        public static T Instance => _instance.Value;

        public abstract void Initialize();
    }
}
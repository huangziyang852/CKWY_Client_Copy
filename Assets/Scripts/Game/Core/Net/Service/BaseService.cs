using System;

namespace Game.Core.Net.Service
{
    public abstract class BaseService<T> where T : BaseService<T>, new()
    {
        private static readonly Lazy<T> _instance = new(() => new T());
        public static T Instance => _instance.Value;
    }
}
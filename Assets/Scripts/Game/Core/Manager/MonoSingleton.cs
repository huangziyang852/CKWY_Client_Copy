using UnityEngine;

namespace Game.Core.Net
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static GameObject _gameObject;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock) // 线程安全
                    {
                        if (_instance == null)
                        {
                            // 先尝试在场景里找
                            _instance = FindObjectOfType<T>();

                            if (_instance == null)
                            {
                                // 没找到就新建一个
                                _gameObject = new GameObject(typeof(T).Name);
                                _instance = _gameObject.AddComponent<T>();
                            }

                            // 保证运行时不被销毁
                            if (Application.isPlaying)
                                DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // 防止重复创建
            if (_instance == null)
            {
                _instance = this as T;
                if (Application.isPlaying)
                    DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject); // 如果已有实例，销毁多余的
            }
        }
    }
}

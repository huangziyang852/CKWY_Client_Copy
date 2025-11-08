using System.Diagnostics;
using UnityEngine;

namespace Game.Core.Manager
{
    public class EventManager : MonoBehaviour
    {
        //定义委托类型
        public delegate void UpdateBeat(); //心跳委托

        private static EventManager _instance;
        public float intervalInSeconds = 1.0f;

        private Stopwatch stopwatch;

        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    if ((_instance = FindObjectOfType<EventManager>()) == null)
                    {
                        var go = new GameObject(typeof(EventManager).ToString());
                        _instance = go.AddComponent<EventManager>();
                    }

                    if (Application.isPlaying) DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        private void Start()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        private void Update()
        {
            //检查计时是否超过间隔时间
            if (stopwatch.Elapsed.TotalSeconds >= intervalInSeconds)
            {
                //触发所有注册到OnUpdateBeat的方法
                OnUpdateBeat?.Invoke();

                //重置计时器
                stopwatch.Restart();
            }
        }

        //定义事件，供外部添加方法
        public static event UpdateBeat OnUpdateBeat; //心跳时间

        //外部使用的接口方法
        public void AddMethodToUpdateBeat(UpdateBeat method)
        {
            OnUpdateBeat += method; //添加方法
        }

        public void RemoveMethodFromUpdateBeat(UpdateBeat method)
        {
            OnUpdateBeat -= method; //移除方法
        }
    }
}
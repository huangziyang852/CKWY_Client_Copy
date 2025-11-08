using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Core.Net
{
    internal class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    public class NetWorkService : MonoBehaviour
    {
        private static NetWorkService _instance;

        public static NetWorkService Instance
        {
            get
            {
                if (_instance == null)
                {
                    if ((_instance = FindObjectOfType<NetWorkService>()) == null)
                    {
                        var go = new GameObject(typeof(NetWorkService).ToString());
                        _instance = go.AddComponent<NetWorkService>();
                    }

                    if (Application.isPlaying) DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        private void Awake()
        {
        }

        public void OnInit()
        {
            //Util.CallMethod("SocketManager", "Start");
        }

        public void Unload()
        {
            //Util.CallMethod("SocketManager", "Unload");
        }

        public void SendGetHttp(string url, Action<string> func, Action errorAction)
        {
            //StartCoroutine(HttpGet_Co(url, callback, sharpFunc, errorAction, errorLuaFunc));
            StartCoroutine(HttpGet_Co(url, func, errorAction));
        }

        private IEnumerator HttpGet_Co(string url, Action<string> func, Action errorAction)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (errorAction != null) errorAction();
                yield break;
            }

            var request = UnityWebRequest.Get(url);
            request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();

            yield return request.SendWebRequest();
            //if (request.isNetworkError)
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("url::" + url + " HttpGet Error:    " + request.error);
                if (errorAction != null)
                    errorAction();
            }
            else
            {
                //var result = Encoding.UTF8.GetString(getData.bytes);
                var result = request.downloadHandler.text;

                if (func != null)
                    func(result);
            }
        }

        public void SendPostHttp(string url, WWWForm postData, Action<string> func, Action errorAction)
        {
            //StartCoroutine(HttpGet_Co(url, callback, sharpFunc, errorAction, errorLuaFunc));
            StartCoroutine(HttpPost_Co(url, postData, func, errorAction));
        }

        private IEnumerator HttpPost_Co(string url, WWWForm postData, Action<string> func, Action errorAction)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (errorAction != null) errorAction();
                yield break;
            }

            var request = UnityWebRequest.Post(url, postData);
            Debug.Log("Request URL: " + request.url);
            Debug.Log("Request Body: " + postData);
            request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();

            yield return request.SendWebRequest();
            //if (request.isNetworkError)
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("url:" + url + " HttpGet Error:    " + request.error);
                if (errorAction != null)
                    errorAction();
            }
            else
            {
                //var result = Encoding.UTF8.GetString(getData.bytes);
                var result = request.downloadHandler.text;

                if (func != null)
                {
                    Debug.Log("Received raw data: " + result);
                    func(result);
                }
            }
        }
    }
}
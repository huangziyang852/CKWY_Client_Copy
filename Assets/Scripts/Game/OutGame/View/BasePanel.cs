using Game.Common.Const;
using UnityEngine;

namespace Game.OutGame.View
{
    public class BasePanel<T> : MonoBehaviour where T : BasePanel<T>
    {
        private static T _instance;
        private static bool _initialized;

        private UISTATE PanelState = UISTATE.CLOSE;
        //private CanvasGroup canvasGroup;

        public static T Instance
        {
            get
            {
                if (_instance == null || _instance.gameObject == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null) Debug.LogError("instance not found in the scene!");
                }

                return _instance;
            }
        }

        public string PanelName
        {
            get => PanelName;
            set => PanelName = value;
        }


        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            //DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public virtual void SetState(UISTATE uiState)
        {
            PanelState = uiState;
            switch (PanelState)
            {
                case UISTATE.CLOSE:
                    ClosePanel();
                    break;
                case UISTATE.OPEN:
                    OpenPanel();
                    break;
                case UISTATE.PAUSED:
                    pausePanel();
                    break;
            }
        }

        public virtual void OpenPanel()
        {
            gameObject.SetActive(true);
            //canvasGroup.interactable = true;
            PanelState = UISTATE.OPEN;
        }

        public virtual void ClosePanel()
        {
            gameObject.SetActive(false);
            PanelState = UISTATE.CLOSE;
        }

        public virtual void pausePanel()
        {
            //canvasGroup.interactable = false;
            PanelState = UISTATE.PAUSED;
            Debug.Log("pause panel:" + name);
        }

        public virtual void resumePanel()
        {
            //canvasGroup.interactable = true;
            PanelState = UISTATE.OPEN;
        }
    }
}
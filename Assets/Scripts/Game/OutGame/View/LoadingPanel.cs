using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.View
{
    public class LoadingPanel : BasePanel<LoadingPanel>
    {
        [SerializeField] private Slider progressBar; // 进度条
        [SerializeField] private Text loadingText; // 提示文本
        [SerializeField] private Image bg;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }


        public void SetProgress(float progress)
        {
            progressBar.value = Mathf.Clamp01(progress);
            //loadingText.text = $"Loading {Mathf.RoundToInt(progress * 100)}%";
        }
    }
}
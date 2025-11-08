using Game.Common.Const;
using Game.Core.Animation;
using Game.Core.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.OutGame.UIComp
{
    public class TabButton : MonoBehaviour
    {
        public Toggle toggle; // 绑定 Toggle 组件
        public Image tabImage; // 绑定 Tab 的 Image（用于切换图片）
        public Sprite selectedSprite; // 选中时的图片
        public Sprite unselectedSprite; // 未选中时的图片 
        public GameObject tabEffectAnimator;
        public UnityEvent onTabSelected; // 选中时执行的方法

        // Start is called before the first frame update
        private void Start()
        {
            // 初始状态
            UpdateTabUI(toggle.isOn);

            // 监听 Toggle 状态变化
            toggle.onValueChanged.AddListener(OnTabChanged);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnTabChanged(bool isOn)
        {
            SoundManager.Instance.PlayClickSound(GameConst.AudioName.UIAudioName.Page);
            UpdateTabUI(isOn);
            if (isOn) onTabSelected?.Invoke(); // 执行绑定的方法
        }

        private void UpdateTabUI(bool isOn)
        {
            tabImage.sprite = isOn ? selectedSprite : unselectedSprite;
            tabEffectAnimator.SetActive(isOn);
        }
    }
}
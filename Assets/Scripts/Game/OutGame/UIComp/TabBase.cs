using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.UIComp
{
    public class TabBase : MonoBehaviour
    {
        public List<TabButton> tabs;
        public ToggleGroup toggleGroup; // Toggle 组
        public Action<int> onTabChanged;

        // Start is called before the first frame update
        private void Start()
        {
            for (var i = 0; i < tabs.Count; i++)
            {
                var index = i; // 闭包变量
                tabs[i].toggle.group = toggleGroup;

                // 添加监听
                tabs[i].toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn) onTabChanged?.Invoke(index); // 选中后触发回调
                });
            }
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void SelectTab(int index)
        {
            if (index >= 0 && index < tabs.Count) tabs[index].toggle.isOn = true;
        }
    }
}
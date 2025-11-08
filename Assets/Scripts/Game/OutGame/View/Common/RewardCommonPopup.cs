using Game.Common.Const;
using Game.Core.Manager;
using Game.Model;
using Game.OutGame.View.Item;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.View.Common
{
    public class RewardCommonPopup : BasePanel<RewardCommonPopup>
    {
        public Button grayOutButton;
        public Transform contentTrans;
        public GameObject itemGridPrefab;
        public TextMeshProUGUI text;
        // Start is called before the first frame update
        void Start()
        {
            SoundManager.Instance.PlayClickSound(GameConst.AudioName.UIAudioName.Union);
            grayOutButton.onClick.AddListener(ClosePanel);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(List<ItemModel> items,string desc)
        {
            foreach (var item in items)
            {
                var itemGridCom = Instantiate(itemGridPrefab, contentTrans).GetComponent<ItemGridCom>();
                itemGridCom.Init(item);
            }
        }

        public override void ClosePanel()
        {
            Destroy(gameObject);
        }
    }
}

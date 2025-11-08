using Game.Common.Utils;
using Game.Core.Manager;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Game.OutGame.View.Common
{
    public class CurrencyItemComp : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI countText;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public async void Init(int itemId)
        {
            Sprite itemSprite = await ResourceUtils.GetItemSprite(itemId);
            icon.sprite = itemSprite;
            var itemModel = GameModelManager.Instance.ItemInfoModel.GetItem(itemId);
            if(itemModel != null )
            {
                itemModel.Count
                    .Subscribe(count =>
                    {
                        countText.text = count.ToString();
                    })
                    .AddTo(this);
            }
            else
            {
                countText.text = 0.ToString();
            }
        }
    }
}

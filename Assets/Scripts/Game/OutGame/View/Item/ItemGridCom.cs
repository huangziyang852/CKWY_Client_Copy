using Game.Common.Utils;
using Game.Core.Manager;
using Game.Model;
using Game.OutGame.View.Hero;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.View.Item
{
    public class ItemGridCom : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI count;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public async void Init(ItemModel itemModel)
        {
            Sprite itemSprite = await ResourceUtils.GetItemSprite(itemModel.ItemId);
            icon.sprite = itemSprite;
            count.text = itemModel.Count.ToString();
        }
    }
}

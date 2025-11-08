using Game.Common.Const;
using Game.Common.Utils;
using Game.Core.Net;
using Game.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Game.Common.Const.GameConst;

namespace Game.Core.Manager
{
    public class BackGroundManager:MonoSingleton<BackGroundManager>
    {
        [SerializeField]
        private Image bgImage;
        [SerializeField]
        private GameObject bgSpineRoot;
        // Start is called before the first frame update
        private void Start()
        {
            
        }

        public async void SetBackGroundImage(BgImageType bgImageType, string imageName)
        {
            bgImage.sprite = null;
            bgImage.enabled = false;
            var sprite = await ResourceUtils.GetBgSprite(bgImageType,imageName);
            if (sprite != null)
            {
                bgImage.sprite = sprite;
                bgImage.SetNativeSize();
                bgImage.enabled = true; 
            }
        }

        public async void SetBackGroundSpine(BgSpineType bgSpineType,string fileName)
        {
            var prefab = await ResourceUtils.GetBgSpinePrefab(bgSpineType, fileName);
            if (prefab != null)
            {
                GameObject.Instantiate(prefab,bgSpineRoot.transform,false);
            }
        }

        public void ClearBackGroundSpine()
        {
            for (int i = bgSpineRoot.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(bgSpineRoot.transform.GetChild(i).gameObject);
            }
        }
    }
}

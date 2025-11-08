using cfg.battle;
using Game.Common.Utils;
using Game.OutGame.UIComp;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Ingame.Comp
{
    public class ChapterItemPre : MonoBehaviour
    {
        public SkeletonAnimation buildingAnimation;
        public SkeletonAnimation fightAnimation;
        public SpriteRenderer buildingSprite;
        public TextMeshPro title;
        public TextMeshPro progress;
        public ClickableObject clickableObject;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void Init(BattleMap battleMap)
        {
            if (battleMap.Build == 0)
            {
                buildingAnimation.gameObject.SetActive(false);
                ResourceUtils.GetMapBuildingSprite(battleMap.Id, sprite =>
                {
                    buildingSprite.gameObject.SetActive(true);
                    buildingSprite.sprite = sprite;
                });
            }
            else
            {
                ResourceUtils.GetMapBuildingSkeletonData(battleMap.Build, skeletonDataAsset =>
                {
                    buildingAnimation.skeletonDataAsset = skeletonDataAsset;
                    buildingAnimation.Initialize(true);
                });
            }

            title.text = battleMap.Name;
        }
    }
}
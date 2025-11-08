using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Game.Common.Const;
using Game.Core.Manager;
using Spine.Unity;
using UnityEngine;
using static Game.Common.Const.GameConst;
using static Game.Common.Const.GameConst.ImagesPath;

namespace Game.Common.Utils
{
    public static class ResourceUtils
    {
        /// <summary>
        /// 获取人物图片
        /// </summary>
        /// <param name="heroImageType"></param>
        /// <param name="heroId"></param>
        /// <param name="callback"></param>
        public static void GetHeroSprite(HeroImageType heroImageType, int heroId, Action<Sprite> callback)
        {
            var heroImagePath = string.Empty;
            switch (heroImageType)
            {
                case HeroImageType.Head:
                {
                    heroImagePath = GameConst.ImagesPath.Character.HERO_ICON + heroId;
                    break;
                }
            }

            ResourceLoader.Instance.LoadRemoteSprite(heroImagePath, callback);
        }

        /// <summary>
        ///  获取地图spine
        /// </summary>
        /// <param name="index"></param>
        /// <param name="callback"></param>
        public static void GetMapBuildingSkeletonData(int index, Action<SkeletonDataAsset> callback)
        {
            var path = GameConst.SpinePath.Battle.Map.BUILDING_SPINE + index + "_SkeletonData";
            ResourceLoader.Instance.LoadRemoteSkeletonData(path, callback);
        }

        public static void GetMapBuildingSprite(int index, Action<Sprite> callback)
        {
            var path = GameConst.ImagesPath.Battle.Map.Icon.BUILDING_ICON + index;
            ResourceLoader.Instance.LoadRemoteSprite(path, callback);
        }

        public static async UniTask<Sprite> GetSpriteByPath(string path)
        {
            return await ResourceLoader.Instance.LoadRemoteSpriteAsync(path);
        }

        public static async UniTask<SkeletonDataAsset> GetSkeletonDataByPath(string path)
        {
            return await ResourceLoader.Instance.LoadRemoteSkeletonData(path);
        }

        public static async UniTask<SkeletonDataAsset> GetHeroSpineSkeletonData(int heroId)
        {
            var heroMaster = TableManager.Instance.MasterTables.TbHero.Get(heroId);
            var path = GameConst.SpinePath.Battle.HERO_SPINE_PATH + heroId + "/"+heroMaster.PathName+"_SkeletonData";
            return await ResourceLoader.Instance.LoadRemoteSkeletonData(path);
        }

        public static void GetHeroSpineSkeletonData(int heroId, Action<SkeletonDataAsset> callback)
        {
            var heroMaster = TableManager.Instance.MasterTables.TbHero.Get(heroId);
            var path = GameConst.SpinePath.Battle.HERO_SPINE_PATH + heroId + "/" + heroMaster.PathName + "_SkeletonData";
            ResourceLoader.Instance.LoadRemoteSkeletonData(path, callback);
        }

        public static async UniTask<SkeletonDataAsset> GetHeroEffectSpineSkeletonData(int heroId,string fileName)
        {
            var heroMaster = TableManager.Instance.MasterTables.TbHero.Get(heroId);
            var path = GameConst.SpinePath.Battle.HERO_ATTACK_EFFECT_SPINE_PATH + heroId + "/" + fileName + "_SkeletonData";
            return await ResourceLoader.Instance.LoadRemoteSkeletonData(path);
        }

        public static async UniTask<AudioClip> GetSoundEffect(string fileName)
        {
            string path = GameConst.SoundPath.Effect.SKILL_SOUND_PATH + fileName;
            return await ResourceLoader.Instance.LoadRemoteAudio(path);
        }

        public static async UniTask<AudioClip> GetUISoundEffect(string fileName)
        {
            string path = GameConst.SoundPath.UI_SOUND_PATH + fileName;
            return await ResourceLoader.Instance.LoadRemoteAudio(path);
        }

        public static async UniTask<SkeletonDataAsset> GetGachaCardSpineSkeletonData(GachaType gachaType)
        {
            string path = "";
            switch (gachaType)
            {
                case GachaType.Normal:
                    path = GameConst.SpinePath.UISpine.Gacha.CARD_NORMAL + "_SkeletonData";
                    break;
                case GachaType.Friend:
                    path = GameConst.SpinePath.UISpine.Gacha.CARD_FRIEND + "_SkeletonData";
                    break;
                case GachaType.Super:
                    path = GameConst.SpinePath.UISpine.Gacha.CARD_SUPER + "_SkeletonData";
                    break;
            }
            return await ResourceLoader.Instance.LoadRemoteSkeletonData(path);
        }

        public static async UniTask<Sprite> GetBgSprite(BgImageType bgImageType, string imageName)
        {
            string path = null;
            switch (bgImageType)
            {
                case BgImageType.Login:
                    path = GameConst.ImagesPath.BG.Login.LOGIN + imageName;
                    break;
                case BgImageType.Home:
                    path = GameConst.ImagesPath.BG.Home.HOME + imageName;
                    break;
                case BgImageType.Gacha:
                    path = GameConst.ImagesPath.BG.Gacha.GACHA + imageName;
                    break;
            } 
            return await ResourceLoader.Instance.LoadRemoteSpriteAsync(path);
        }

        public static async UniTask<Sprite> GetHeroSprite(HeroImageType heroImageType,int heroId)
        {
            string path = null;
            switch (heroImageType)
            {
                case HeroImageType.Stand:
                    path = GameConst.ImagesPath.Character.HERO_STAND + heroId;
                    break;
            }
            return await ResourceLoader.Instance.LoadRemoteSpriteAsync(path);
        }

        public static async UniTask<Sprite> GetItemSprite(int itemId)
        {
            var itemMaster = TableManager.Instance.MasterTables.TbItem.Get(itemId);
            string path = GameConst.ImagesPath.UI.ITEM + itemMaster.Icon;
            return await ResourceLoader.Instance.LoadRemoteSpriteAsync(path);
        }

        public static async UniTask<Sprite> GetCampIconSprite(int camp)
        {
            var iconPath = string.Empty;
            switch (camp)
            {
                case 1:
                    iconPath = GameConst.ImagesPath.UI.Hero.Camp.CAMP_RED;
                    break;
                case 2:
                    iconPath = GameConst.ImagesPath.UI.Hero.Camp.CAMP_BLUE;
                    break;
                case 3:
                    iconPath = GameConst.ImagesPath.UI.Hero.Camp.CAMP_GREEN;
                    break;
                case 4:
                    iconPath = GameConst.ImagesPath.UI.Hero.Camp.CAMP_YELLOW;
                    break;
                case 5:
                    iconPath = GameConst.ImagesPath.UI.Hero.Camp.CAMP_PURPLE;
                    break;
            }
            return await ResourceUtils.GetSpriteByPath(iconPath);
        }

        public static async UniTask<Sprite> GetRarityIconSprite(int rarity)
        {
            var iconPath = string.Empty;
            switch (rarity)
            {
                case 1:
                    iconPath = GameConst.ImagesPath.UI.Hero.Rarity.N;
                    break;
                case 2:
                    iconPath = GameConst.ImagesPath.UI.Hero.Rarity.R;
                    break;
                case 3:
                    iconPath = GameConst.ImagesPath.UI.Hero.Rarity.SR;
                    break;
                case 4:
                    iconPath = GameConst.ImagesPath.UI.Hero.Rarity.SSR;
                    break;
            }
            return await ResourceUtils.GetSpriteByPath(iconPath);
        }

        public static async UniTask<GameObject> GetBgSpinePrefab(BgSpineType bgSpineType, string fileName)
        {
            string path = null;
            switch(bgSpineType)
            {
                case BgSpineType.Login:
                    path = GameConst.PrefabPath.LOGIN + fileName;
                    break;
                case BgSpineType.Gacha:
                    path = GameConst.PrefabPath.GACHA + fileName;
                    break;
            }

            return await ResourceLoader.Instance.LoadRemotePrefabAsync(path);
        }
    }
}
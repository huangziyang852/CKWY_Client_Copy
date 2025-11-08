using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Core.Net.Service;
using Game.Model;
using World = Game.Model.World;

namespace Game.Core.Manager
{
    public class GameModelManager
    {
        private static GameModelManager _instance;

        private GameModelManager()
        {
        }

        public PlayerInfoModel PlayerInfoModel { get; private set; }

        public HeroInfoModel HeroInfoModel { get; private set; }

        public WorldInfoModel WorldInfoModel { get; private set; }

        public ItemInfoModel ItemInfoModel { get; private set; }

        public static GameModelManager Instance
        {
            get { return _instance ??= new GameModelManager(); }
        }

        /**
         * 初始化用户数据
         */
        public async void InitUserData()
        {
        }

        /**
         * 向服务器请求数据
         * */
        public async UniTask<PlayerInfoModel> GetPlayerInfoFromServerAsync()
        {
            if (PlayerInfoModel == null) PlayerInfoModel = await FetchPlayerInfoFromServerAsync();
            return PlayerInfoModel;
        }

        private async Task<PlayerInfoModel> FetchPlayerInfoFromServerAsync()
        {
            var response = await UserService.Instance.GetPlayerInfoAsync();
            var deckList = response.Deck.Select(battleSlot => new BattleSlot
            (
                battleSlot.Position,
                battleSlot.HeroCd
            )).ToList();
            return new PlayerInfoModel(response.UserId, response.OpenId, response.UserName,
                response.Level, response.Exp, response.Gold, response.Diamond,deckList);
        }

        public async UniTask<HeroInfoModel> GetHeroInfoFromServerAsync()
        {
            if (HeroInfoModel == null) HeroInfoModel = await FetchHeroInfoFromServerAsync();
            return HeroInfoModel;
        }

        //public async Task RefreshHeroInfoAsync()
        //{
        //    HeroInfoModel = await FetchHeroInfoFromServerAsync();
        //}

        private async Task<HeroInfoModel> FetchHeroInfoFromServerAsync()
        {
            var response = await HeroService.Instance.GetHeroInfoAsync();
            return new HeroInfoModel
            {
                Heroes = response.Heroes
                    .Select(h => new HeroModel(
                        h.HeroId,
                        h.HeroCd,
                        h.Level,
                        h.Star,
                        h.Quality
                    ))
                    .ToList()
            };
        }

        public async UniTask<WorldInfoModel> GetWorldInfoFromServerAsync()
        {
            if (WorldInfoModel == null)
            {
                var response = await WorldService.Instance.GetWorldInfoAsync();
                WorldInfoModel = new WorldInfoModel
                {
                    Worlds = response.Worlds.Select(world => new World
                    {
                        WolrdId = world.WorldId
                    }).ToList()
                };
            }

            return WorldInfoModel;
        }

        public async UniTask<ItemInfoModel> GetItemInfoFromServerAsync()
        {
            if (ItemInfoModel == null) ItemInfoModel = await FetchItemInfoFromServerAsync();
            return ItemInfoModel;
        }

        private async Task<ItemInfoModel> FetchItemInfoFromServerAsync()
        {
            var response = await ItemService.Instance.GetItemInfoAsync();
            return new ItemInfoModel
            {
                Items = response.Items
                    .Select(item => new ItemModel(item.ItemId, item.ItemCount))
                    .ToList()
            };
        }

        /**
         * 工具方法
         * */
        public HeroModel? GetHeroModelByHeroCd(string heroCd)
        {
            return HeroInfoModel.Heroes.Find(h => h.HeroCd == heroCd);
        }
    }
}
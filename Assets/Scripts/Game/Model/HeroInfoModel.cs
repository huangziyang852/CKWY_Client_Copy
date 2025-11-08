using LaunchPB;
using System.Collections.Generic;
using System.Linq;

namespace Game.Model
{
    public class HeroInfoModel
    {
        public List<HeroModel> Heroes { get; set; } = new();

        /// <summary>
        /// 添加或更新英雄
        /// </summary>
        public void AddOrUpdateHero(Hero hero)
        {
            var heroModel = Heroes.FirstOrDefault(h => h.HeroCd == hero.HeroCd);

            if (heroModel != null)
            {
                heroModel.Star = hero.Star;
                heroModel.Level = hero.Level;
                heroModel.Quality = hero.Quality;
            }
            else
            {
                var newHero = new HeroModel(hero.HeroId,hero.HeroCd,hero.Level,hero.Star,hero.Quality);
                Heroes.Add(newHero);
            }
        }
    }

    public class HeroModel:IBaseModel
    {
        public int HeroId { get; }
        public string HeroCd { get; }
        public int Level { get; set; }
        public int Star { get; set; }
        public int Quality { get; set; }

        public HeroModel(int heroId,string heroCd,int level,int star, int quality)
        {
            HeroId = heroId;
            HeroCd = heroCd;
            Level = level;
            Star = star;
            Quality = quality;
        }
    }
}
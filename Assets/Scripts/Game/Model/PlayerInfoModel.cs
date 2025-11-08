using System.Collections.Generic;

namespace Game.Model
{
    public class PlayerInfoModel:IBaseModel
    {
        public PlayerInfoModel()
        {
            UsertId = "";
            OpenId = 0;
            Name = "default";
            Level = 0;
            Exp = 0;
            Gold = 0;
            Diamond = 0;
            Deck = new List<BattleSlot>();
        }

        public PlayerInfoModel(string usertId, long openId, string name, int level, int exp, int gold, int diamond, List<BattleSlot> deck)
        {
            UsertId = usertId;
            OpenId = openId;
            Name = name;
            Level = level;
            Exp = exp;
            Gold = gold;
            Diamond = diamond;
            Deck = deck;
        }

        public string UsertId { get; set; }

        public long OpenId { get; set; }
        public string Name { get; set; }

        public int Level { get; set; }
        public int Exp { get; set; }

        public int Gold { get; set; }

        public int Diamond { get; set; }

        public List<BattleSlot> Deck = new List<BattleSlot>();
    }

    public class BattleSlot
    {
        public int Position { get; set; }
        public string HeroCd { get; set; }

        public BattleSlot(int position, string heroCd)
        {
            Position = position;
            HeroCd = heroCd;
        }
    }
}
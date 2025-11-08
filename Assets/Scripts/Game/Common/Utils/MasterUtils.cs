using cfg.hero;
using cfg.skill;
using Game.Core.Manager;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Common.Utils
{
    public class MasterUtils
    {
        public static Skill GetSkillMasterByHeroModel(HeroModel heroModel)
        {
            HeroAttr heroAttr = TableManager.Instance.MasterTables.TbHeroAttr.DataList.Find((heroAttr) =>
            {
                return heroAttr.HeroID == heroModel.HeroId && heroAttr.Star == heroModel.Star;
            });
            return TableManager.Instance.MasterTables.TbSkill.Get(heroAttr.NormalAtkID);
        }
    }
}

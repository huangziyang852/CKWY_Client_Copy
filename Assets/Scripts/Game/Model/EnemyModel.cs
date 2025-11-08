using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class EnemyModel:IBaseModel
    {
        public int EnemyId { get; set; }
        public int Star { get; set; }

        public EnemyModel(int enemyId,int star) 
        {
            EnemyId = enemyId;
            Star = star;
        }
    }
}


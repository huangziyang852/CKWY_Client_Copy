using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame
{
    public class BattleConst
    {
        public static readonly int DEF_KEY = 101;
        public static readonly int HP_KEY = 102;
        public static readonly int ATK_KEY = 103;
        public static readonly int SPD_KEY = 104;

        public static readonly Vector3[] PLAYER_UNIT_POSITION = new Vector3[] { new Vector3(-13.0f, -2.0f, 0), new Vector3(0, -1, 0), new Vector3(0, -2, 0), new Vector3(2, -1.5f, 0), new Vector3(2, -0.5f, 0) };

        public static float ATTACK_INTERVAL = 2.5f;
        public static float BULLET_SPEED = 28.0f;
    }
}

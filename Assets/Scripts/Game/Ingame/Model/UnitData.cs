using Game.Ingame.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.Model
{
    public class UnitData
    {
        public Vector3 Position { get; set; }

        public UnitController UnitController { get; set; }
    }
}

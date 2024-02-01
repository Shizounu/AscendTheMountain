using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "new Unit", menuName = "Cards/Unit")]
    public class UnitDefinition : CardDefinition
    {
        public int Health;
        public int Attack;
        public int MoveDistance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cards;

namespace Combat
{
    public class Unit {
        public Unit(UnitDefinition definition) {

        }

        public int Health;
        public int MaxHealth;

        public int Attack;

        public int MoveSpeed;

        public List<IEffect> effect;
    }
}

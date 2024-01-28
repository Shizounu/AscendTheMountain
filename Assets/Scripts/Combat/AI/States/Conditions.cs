using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.AI.StateMachine
{
    public abstract class Condition : ScriptableObject {
        public int Weight = 1;
        public abstract int Evaluate(Board board);
    }
}

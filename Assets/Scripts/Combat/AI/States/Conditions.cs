using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.AI.StateMachine
{
    public abstract class Condition : ScriptableObject {
        public abstract int Evaluate(Board board);
    }
}

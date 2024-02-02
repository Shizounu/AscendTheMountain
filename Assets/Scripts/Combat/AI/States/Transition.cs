using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.AI.StateMachine
{
    [System.Serializable]
    public struct Transition {
        public List<ConditionThresholdPair> TransitionConditions;
        [Space()]
        public State transitionTo;

        public bool Evaluate(BoardInfo board) {
            bool cur = true;
            int i = 0;
            do //this is intended to cycle through the entire list of conditions. If any evaluate to false, stop the list and return false. Else return true
            { 
                cur = cur && TransitionConditions[i].Evaluate(board);
            } while (cur && i < TransitionConditions.Count);
            return cur;
        }
    }

    [System.Serializable]
    public struct ConditionThresholdPair {
        public Condition condition;
        public int threshhold;
        public bool invert;

        public bool Evaluate(BoardInfo board) {
            return (condition.Evaluate(board) >= threshhold) != invert;
        }
    }
}

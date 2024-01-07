using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.AI.StateMachine
{
    [CreateAssetMenu(fileName = "new AI State", menuName = "AI/State")]
    public class State : ScriptableObject
    {
        public List<Condition> boardEvaluationConditions;

        public List<Transition> transitions;
    }

}

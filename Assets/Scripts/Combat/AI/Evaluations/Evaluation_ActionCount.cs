using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Combat.AI.StateMachine
{
    [CreateAssetMenu(fileName = "new ActionCount", menuName = "AI/Conditions/Action Count")]
    public class Evaluation_ActionCount : Condition
    {
        public override int Evaluate(BoardInfo board)
        {
            return board.actionsTaken.Count;
        }
    }

}

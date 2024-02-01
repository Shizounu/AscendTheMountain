using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.AI.StateMachine
{
    [CreateAssetMenu(fileName = "new Heat", menuName = "AI/Conditions/Heat")]
    public class Evaluation_Heat : Condition {
        public int HeatAmplitude; 

        public override int Evaluate(Board board) {
            return Random.Range(-HeatAmplitude, HeatAmplitude);
        }
    }

}

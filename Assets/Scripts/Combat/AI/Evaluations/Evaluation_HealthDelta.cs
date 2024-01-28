using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.AI.StateMachine
{
    [CreateAssetMenu(fileName = "new HealthDelta", menuName = "AI/Conditions/Health Delta")]
    public class Evaluation_HealthDelta : Condition {
        public override int Evaluate(Board board) {
            int playerHealth = 0;
            List<Vector2Int> playerUnitPositions = board.GetUnitPositions(Actors.Actor1);
            foreach (var unit in playerUnitPositions)
                playerHealth += board.GetUnitFromPos(unit).curHealth;

            int AIHealth = 0;
            List<Vector2Int> AIUnitHealth= board.GetUnitPositions(Actors.Actor1);
            foreach (var unit in AIUnitHealth)
                AIHealth += board.GetUnitFromPos(unit).curHealth;

            return playerHealth - AIHealth;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.AI.StateMachine
{
    [CreateAssetMenu(fileName = "new HealthDelta", menuName = "AI/Conditions/Health Delta")]
    public class Evaluation_HealthDelta : Condition {
        public override int Evaluate(BoardInfo boardInfo) {
            int playerHealth = 0;
            List<Vector2Int> playerUnitPositions = boardInfo.board.GetUnitPositions(Actors.Actor1);
            foreach (var unit in playerUnitPositions)
                playerHealth += boardInfo.board.GetUnitReference(unit).unitReference.curHealth;

            int AIHealth = 0;
            List<Vector2Int> AIUnitHealth= boardInfo.board.GetUnitPositions(Actors.Actor1);
            foreach (var unit in AIUnitHealth)
                AIHealth += boardInfo.board.GetUnitReference(unit).unitReference.curHealth;

            return playerHealth - AIHealth;
        }
    }
}

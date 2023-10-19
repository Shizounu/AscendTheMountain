using Cards;
using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Combat{
    public enum Phase {
        PlayerTurn,
        AITurn,

        ResetTurn
    }

    public class GameManager : MonoBehaviour {

        [Header("Phase Control")]
        public Phase CurrentPhase;
        public UnityEvent<Phase> onPhaseChange;
        
        [Header("Summoning")]
        public UnitController unitController;

        [Header("references")]
        public Tile[,] currentMap;
        public MapGenerator mapGenerator;
        private void Start() {
            CurrentPhase = Phase.PlayerTurn;
            currentMap = mapGenerator.GenerateMap();

            onPhaseChange.Invoke(CurrentPhase); //initialize side
        }




        public void IncrementPhase() {
            CurrentPhase += 1; //increments to the next phase in order

            if (CurrentPhase == Phase.ResetTurn) //if final phase is reached, reset to the beginning of a turn
                CurrentPhase = 0;

            onPhaseChange.Invoke(CurrentPhase);

        }
        public void SummonUnit(UnitDefinition definition, Vector2Int position, SideManager manager){
            UnitController unit = Instantiate(unitController, currentMap[position.x,position.y].transform.position, Quaternion.identity, manager.transform);
            unit.Initialize(definition, position, manager);


        }
    }
}

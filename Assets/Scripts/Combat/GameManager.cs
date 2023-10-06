using Cards;
using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat{
    public enum Phase {
        PlayerTurn,
        AITurn
    }

    public class GameManager : MonoBehaviour {
        public Tile[,] currentMap;

        [Header("Phase Control")]
        public Phase CurrentPhase;
        [Header("Summoning")]
        public UnitController unitController;

        [Header("references")]
        public MapGenerator mapGenerator;
        private void Start() {
            CurrentPhase = Phase.PlayerTurn;
            currentMap = mapGenerator.GenerateMap();
        }
        public void IncrementPhase() {
            
        }
        public void SummonUnit(UnitDefinition definition, Vector2Int position, SideManager manager){
            UnitController unit = Instantiate(unitController, currentMap[position.x,position.y].transform.position, Quaternion.identity, manager.transform);
            unit.Initialize(definition, position, manager);


        }
    }
}

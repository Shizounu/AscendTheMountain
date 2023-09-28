using Cards;
using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat{
    public class GameManager : MonoBehaviour {
        public Tile[,] currentMap;

        [Header("Summoning")]
        public UnitController unitController;

        [Header("references")]
        public MapGenerator mapGenerator;
        private void Start() {
            currentMap = mapGenerator.GenerateMap();
        }

        public void SummonUnit(UnitDefinition definition, Vector2Int position, SideManager manager){
            UnitController unit = Instantiate(unitController, currentMap[position.x,position.y].transform.position, Quaternion.identity, manager.transform);
            unit.Initialize(definition, position, manager);


        }
    }
}

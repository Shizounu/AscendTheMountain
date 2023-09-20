using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat{
    public class GameManager : MonoBehaviour {
        public Tile[,] currentMap;

        [Header("references")]
        public MapGenerator mapGenerator;
        private void Start() {
            currentMap = mapGenerator.GenerateMap();
        }
    }
}

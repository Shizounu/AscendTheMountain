using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class will hold the actual commands to execute on the battlefield as well as the battlefield itself
namespace Battlefield{
    public class Board : MonoBehaviour
    {
        [Header("Map Gen Settings")]
        public Vector2Int mapDimensions;
        public Vector3 mapRootOffset;

        [Header("Prefabs and References")]
        public Tile tilePrefab;
        public Tile[,] tiles;

        public void generateMap(){
                tiles = new Tile[mapDimensions.x, mapDimensions.y];
                for (int x = 0; x < mapDimensions.x; x++){
                    for (int y = 0; y < mapDimensions.y; y++){
                        tiles[x,y] = Instantiate(tilePrefab, new Vector3(x,y) + mapRootOffset, Quaternion.identity);
                        tiles[x,y].transform.parent = transform;
                        tiles[x,y].gridPosition = new Vector2Int(x,y);
                    }
                }
        }

        private void summonUnit(CardDefinition_Unit unit, Vector2Int position){
            // 1. Create unit object from definition

            // 2. Assign it to the tile 
        }

        private void Start() {
            generateMap();
        }

        private void OnDrawGizmos() {
            for (int x = 0; x < mapDimensions.x; x++){
                for (int y = 0; y < mapDimensions.y; y++){
                    Gizmos.DrawWireCube(new Vector3(x,y) + mapRootOffset, new Vector3(1,1));
                }
            }
        }
    }

}


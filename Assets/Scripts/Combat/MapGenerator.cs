using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Combat {
    public class MapGenerator : MonoBehaviour{
        [Header("Map Definition")]
        public Vector2Int mapDimensions = new Vector2Int(9,5);
        public Vector2 offset = new Vector2(-4.5f, -2.5f);
        public Vector2 tileScale = new Vector2(1.5f, 1.5f);
        [Header("Prefabs")]
        public Tile prefab_Tile;
    
        [ContextMenu("Generate Map")]
        public Tile[,] GenerateMap(){
            Tile[,] result = new Tile[mapDimensions.x, mapDimensions.y];

            for (int x = 0; x < mapDimensions.x; x++){
                for (int y = 0; y < mapDimensions.y; y++){
                    Vector2 position = new Vector2(
                        transform.position.x + x * tileScale.x + offset.x,
                        transform.position.y + y * tileScale.y + offset.y
                    );

                    Tile instance = Instantiate(prefab_Tile, new Vector3(position.x, position.y, 0), Quaternion.identity, this.transform);
                }
            }

            for (int x = 0; x < mapDimensions.x; x++){
                for (int y = 0; y < mapDimensions.y; y++){
                    result[x,y].Init(new Vector2Int(x,y), result);
                }
            }

            return result;
        }

        private void OnDrawGizmos() {
            for (int x = 0; x < mapDimensions.x; x++){
                for (int y = 0; y < mapDimensions.y; y++){
                    Vector2 position = new Vector2(
                        transform.position.x + x * tileScale.x + offset.x,
                        transform.position.y + y * tileScale.y + offset.y
                    );
                    Gizmos.DrawWireCube(new Vector3(position.x, position.y, 0), Vector3.one);
                    Handles.Label(new Vector3(position.x - 0.25f, position.y, 0), $"({x},{y})");
                }

            }
        }
    }
}

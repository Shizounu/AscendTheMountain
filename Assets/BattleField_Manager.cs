using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlefield
{
    public class BattleField_Manager : MonoBehaviour
    {
        [Header("Stats")]
        public Vector2Int hoveredTile;
        [SerializeField] private Vector2Int battleFieldDimensions = new Vector2Int(5,9);
        [SerializeField] private Vector3 positionOffset;


        [Header("References")]
        [SerializeField] private BattleField_Tile tilePrefab;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private BattleField_Tile[,] battleField;
        private void Start() {
            gameManager = GameManager.Instance;

            GenerateField();
        }

        [ContextMenu("Generate Field")]
        private void GenerateField(){
            battleField = new BattleField_Tile[battleFieldDimensions.x, battleFieldDimensions.y];

            for (int x = 0; x < battleFieldDimensions.x; x++){
                for (int y = 0; y < battleFieldDimensions.y; y++){
                    BattleField_Tile instantiatedTile = Instantiate(tilePrefab, new Vector3(x,y) + positionOffset, Quaternion.identity);
                    instantiatedTile.transform.parent = this.transform;
                    instantiatedTile.manager = this;
                    instantiatedTile.tileCord = new Vector2Int(x,y);
                    battleField[x,y] = instantiatedTile;
                }
            }
        }
    }
}

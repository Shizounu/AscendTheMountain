using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Battlefield
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BattleField_Tile : MonoBehaviour
    {
        public BattleField_Manager manager;
        public Vector2Int tileCord;

        private void OnMouseEnter() {
            manager.hoveredTile = tileCord;
        }
        private void OnMouseOver() {
            
        }
        private void OnMouseExit() {
            manager.hoveredTile = new Vector2Int(int.MaxValue, int.MaxValue);
        }
    }
    
}

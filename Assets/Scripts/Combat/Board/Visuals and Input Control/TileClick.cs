using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

namespace Combat
{
    public class TileClick : MonoBehaviour, IPointerClickHandler {
        public Vector2Int position;

        public void OnPointerClick(PointerEventData eventData) {
            PlayerActorManager.Instance.OnTileSelect(position);
        }

        private void OnDrawGizmos() {
            //Debug mark if a unit is on tile
            if (GameManager.Instance.currentBoard.tiles[position.x, position.y].unit != null) {
            
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 1f);

            }
        }
    }
}

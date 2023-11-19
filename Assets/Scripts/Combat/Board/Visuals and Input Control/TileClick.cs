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
    }
}

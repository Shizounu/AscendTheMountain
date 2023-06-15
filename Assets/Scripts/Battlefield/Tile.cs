using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battlefield{
    public enum SelectionState{
        none, 
        hovered, 
        selected
    }

    public class Tile : MonoBehaviour
    {
        public Vector2Int gridPosition;

        [Header("Visual")]
        public SpriteRenderer spriteRenderer;
        public SelectionState selectionState = SelectionState.none;
        public Sprite defaultSprite;
        public Sprite hoveredSprite;
        public Sprite selectedSprite;


        private void OnMouseOver() {
            if(selectionState != SelectionState.selected){
                selectionState = SelectionState.hovered;
            }
        }
        private void OnMouseDown() {

        }
        private void OnMouseExit() {
            if(selectionState != SelectionState.selected){
                selectionState = SelectionState.none;
            }
        }


        private void Update() {
            switch (selectionState)
            {
                case SelectionState.none : 
                    spriteRenderer.sprite = defaultSprite;
                    break;
                
                case SelectionState.hovered : 
                    spriteRenderer.sprite = hoveredSprite;
                    break;
                case SelectionState.selected : 
                    spriteRenderer.sprite = selectedSprite;
                    break;
            }
        }
    }

}


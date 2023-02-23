using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class TileVisualizer : MonoBehaviour
{
    [Header("Selected Visual Info")]
    public Sprite selectedSprite;
    public Color selectedColor;
    [Header("Unselected Visual Info")]
    public Sprite unselectedSprite;
    public Color unselectedColor;

    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private BoxCollider2D _collider;

    private void Awake() {
        if(_spriteRenderer == null){
            //initialize render
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if(_collider == null){
            //initialize collider
            _collider = GetComponent<BoxCollider2D>();
            _collider.isTrigger = true;
        }
        _spriteRenderer.sprite = unselectedSprite;
        _spriteRenderer.color = unselectedColor;
    }
    
    private void OnMouseEnter() {
       onSelect();
    }
    private void OnMouseOver() {
        
    }
    private void OnMouseExit() {
        onUnselect();
    }
    private void OnMouseDown() {
        
    }

    private void onSelect(){
        _spriteRenderer.sprite = selectedSprite;
        _spriteRenderer.color = selectedColor;
    }
    private void onUnselect(){
        _spriteRenderer.sprite = unselectedSprite;
        _spriteRenderer.color = unselectedColor;
    }
}

using HandButtonStates;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class Hand_HandButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public int handIndex;
    
    
    [Space(), Header("Visuals")]
    [SerializeReference, Editor.SubclassPicker] public HandButtonVisualState currentState;
    [Space()]
    public UnityEngine.UI.Image backgroundRenderer;
    public UnityEngine.UI.Image gemRenderer;
    public TMPro.TextMeshProUGUI costText;

    private bool isHovered = false;
    public void OnPointerClick(PointerEventData eventData) {
        if (PlayerActorManager.Instance.Hand[handIndex] != null) {
            PlayerActorManager.Instance.currentState = new InputStates.InputState_HandCardSelected(handIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
    }

    private void Update() {
        if (PlayerActorManager.Instance.isEnabled)
            currentState?.DrawBase(this);
        else
            currentState?.DrawDisabled(this);
        if (isHovered)
            currentState?.OnHover(this);
    }
}

///TODO: Set up listeners to Input System for input

namespace HandButtonStates {
    [System.Serializable]
    public abstract class HandButtonVisualState {
        public Sprite sprite;
        public abstract void DrawBase(Hand_HandButton handButton);
        public abstract void DrawDisabled(Hand_HandButton handButton);
        public abstract void OnHover(Hand_HandButton handButton);
        public abstract void OnClick(Hand_HandButton handButton);
    }
}
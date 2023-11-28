using Combat;
using HandButtonStates;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
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
    public UnityEngine.UI.Image glowRenderer;
    public UnityEngine.UI.Image iconRenderer;
    public TMPro.TextMeshProUGUI costText;

    [SerializeField] private bool isHovered = false;
    private void Start()
    {
        currentState = new HandButtonStates.HandButtonDefault();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (PlayerActorManager.Instance.deckInformation.Hand[handIndex] != null) {
            PlayerActorManager.Instance.currentState = new InputStates.InputState_HandCardSelected(handIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
    }

    // TODO: Put in something other than update
    private void Update() {
        
        currentState?.DrawBase(this);
        if (isHovered)
            currentState?.OnHover(this);
    }
}

///TODO: Set up listeners to Input System for input

namespace HandButtonStates {
    [System.Serializable]
    public abstract class HandButtonVisualState {
        public abstract void DrawBase(Hand_HandButton handButton);
        public abstract void OnHover(Hand_HandButton handButton);
        public abstract void OnClick(Hand_HandButton handButton);
    }

    public class HandButtonDefault : HandButtonVisualState {
        [SerializeField] private float glowValue;
        private static float glowIncrease = .75f;
        private static float glowDecrease = .5f;
        public override void DrawBase(Hand_HandButton handButton)
        {

            if(GameManager.Instance.currentBoard.Actor1_Deck.Hand[handButton.handIndex] != null) {
                handButton.costText.text = $"{GameManager.Instance.currentBoard.Actor1_Deck.Hand[handButton.handIndex].Cost}";
            } else {
                handButton.costText.text = "X"; //TODO Change to nothing later, or leave it - could be nice
            }

            //animate the hand unit
            if(GameManager.Instance.currentBoard.Actor1_Deck.Hand[handButton.handIndex] != null) {
                handButton.iconRenderer.sprite = GameManager.Instance.currentBoard.Actor1_Deck.Hand[handButton.handIndex].Icon;
                handButton.iconRenderer.color = new Color(
                    handButton.iconRenderer.color.r,
                    handButton.iconRenderer.color.g,
                    handButton.iconRenderer.color.b,
                    1
                );
            } else {
                handButton.iconRenderer.sprite = null;
                handButton.iconRenderer.color = new Color(
                    handButton.iconRenderer.color.r,
                    handButton.iconRenderer.color.g,
                    handButton.iconRenderer.color.b,
                    0
                );
            }

            
            //lower bound check
            if (glowValue > 0)
                glowValue -= glowDecrease * Time.deltaTime;
            else
                glowValue = 0;

            //upper bound check
            if (glowValue > 1)
                glowValue = 1;

            handButton.glowRenderer.color = new Color(
                handButton.glowRenderer.color.r,
                handButton.glowRenderer.color.g,
                handButton.glowRenderer.color.b,
                glowValue
            );

        }

        public override void OnClick(Hand_HandButton handButton)
        {
            
        }

        public override void OnHover(Hand_HandButton handButton)
        {
            glowValue += (glowDecrease + glowIncrease) * Time.deltaTime;
        }
    }
}
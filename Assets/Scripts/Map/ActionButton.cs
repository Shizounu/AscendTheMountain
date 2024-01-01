using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using TMPro;
namespace Map.Events
{
    public class ActionButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Visuals")]
        [SerializeField] private float timeTillFullGlow = 1f;

        [Header("References")]
        [SerializeField] private Image glowRenderer;
        [SerializeField] private HoverState hoverState;

        [SerializeField] private TextMeshProUGUI buttonText;
        public UnityEvent callback;

        float hoverTime;
        bool isTriggered = false;

        public void ChangeButtonText(string text){
            buttonText.text = text;
        }

        private void Update() {
            switch (hoverState)
            {
                case HoverState.unselected : 
                    hoverTime = 0;
                    glowRenderer.color = new Color(
                        glowRenderer.color.r,
                        glowRenderer.color.g,
                        glowRenderer.color.b,
                        0  
                    );
                    break;
                case HoverState.hovered : 
                    hoverTime += Time.deltaTime;
                    glowRenderer.color = new Color(
                        glowRenderer.color.r,
                        glowRenderer.color.g,
                        glowRenderer.color.b,
                        Mathf.Lerp(0,1,hoverTime)  
                    );
                    break;
                case HoverState.selected : 
                    if(!isTriggered){
                        isTriggered = true;
                        callback.Invoke();
                    }
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            hoverState = HoverState.selected;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverState = HoverState.hovered;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverState = HoverState.unselected;
            isTriggered = false;
        }
    }
}

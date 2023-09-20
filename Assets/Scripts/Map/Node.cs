using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.Events;
namespace Map
{
    public class Node : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Visualization")]
        [SerializeField] private float timeTillFullGlow = 1f;
        [SerializeField] private float hoverTime = 0;
        [SerializeField] private HoverState hoverState;
        [SerializeField] private Color selectedColor = Color.red;

        [Header("Logic")]
        public bool isInitial;
        public List<Node> connectedNodes;

        [Header("References")]
        [SerializeField] private SpriteRenderer glowRenderer;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Transform ConnectionRenderer;

        [Header("Callback")]
        public UnityEvent onActivate;

        [Header("Prefab")]
        [SerializeField] private LineRenderer connectionLine;

        public void Init(List<Node> nodesToConnect)
        {
            gameManager = FindObjectOfType<GameManager>();
            foreach (Node node in nodesToConnect)
            {
                LineRenderer lineRenderer = Instantiate(connectionLine, transform.position, Quaternion.identity, ConnectionRenderer);
                lineRenderer.SetPositions(new Vector3[]{
                    transform.position,
                    node.transform.position
                });
            }
            connectedNodes = nodesToConnect;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if ((gameManager.currentNode == null && isInitial) || (gameManager.currentNode != null && gameManager.currentNode.connectedNodes.Contains(this)))
            {
                //is allowed to be interacted with
                hoverState = HoverState.selected;
                gameManager.currentNode = this;
                onActivate.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverState != HoverState.selected)
                hoverState = HoverState.hovered;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverState != HoverState.selected)
                hoverState = HoverState.unselected;
        }

        private void Update()
        {
            switch (hoverState)
            {
                case HoverState.unselected:
                    hoverTime = 0;
                    glowRenderer.color = new Color(
                        glowRenderer.color.r,
                        glowRenderer.color.g,
                        glowRenderer.color.b,
                        0
                    );
                    break;
                case HoverState.hovered:
                    hoverTime += Time.deltaTime;
                    glowRenderer.color = new Color(
                        glowRenderer.color.r,
                        glowRenderer.color.g,
                        glowRenderer.color.b,
                        Mathf.Lerp(0, 1, hoverTime)
                    );
                    break;
                case HoverState.selected:
                    glowRenderer.color = selectedColor;
                    break;
            }
        }
    }
}

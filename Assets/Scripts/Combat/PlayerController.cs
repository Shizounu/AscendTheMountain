using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(SideManager))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SideManager sideManager;

        [Header("Selected index")]
        [SerializeField] private int currentHandIndex = -1;

        private void Start() {
            for (int x = 0; x < sideManager.GameManager.currentMap.GetLength(0); x++){
                for (int y = 0; y < sideManager.GameManager.currentMap.GetLength(1); y++){
                    sideManager.GameManager.currentMap[x, y].onTileClick.AddListener(SummonHandCard);
                }
            }
        }


        public void SummonHandCard(Vector2Int mapPosition){
            if(currentHandIndex == -1) {
                Debug.LogError("No card selected");
                return;
            }
            sideManager.PlayCard(sideManager.Hand[currentHandIndex], mapPosition);

            currentHandIndex = -1; //unselects card
        }


        public void SelectHandCard(int index)
        {
            currentHandIndex = index;
        }
    }
}

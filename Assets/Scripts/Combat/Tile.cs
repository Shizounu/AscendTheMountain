using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Shizounu.Library.AI;
using System.Numerics;
using Map;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Combat {
    public class Tile : MonoBehaviour, IAStarTile, IPointerClickHandler
    {
        public UnitController asossciatedUnit = null;
        public Vector2Int position;

        public UnityEvent<Vector2Int> onTileClick;
        /// <summary>
        /// TODO make dependant on potential hazards reducing movespeed
        /// Make dependant on if it can be traveresed
        /// </summary>
        public float TraversalCost { get => 1; set => throw new System.NotImplementedException(); }
        public List<IAStarTile> Adjacencies { get; set; }
        public float Heuristic(IAStarTile goal)
        {
            return 1f;
        }

        public void Init(Vector2Int pos, Tile[,] map){
            #region Adjacencies
            static bool IsExist(Tile[,] mapCells, Vector2Int index){
                bool tileExists = index.x >= 0 &&
                       index.y >= 0 &&
                       index.x < mapCells.GetLength(0) &&
                       index.y < mapCells.GetLength(1);
                return tileExists;
            }
     

            Adjacencies = new();

            Vector2Int curPos = new();

            curPos = pos + Vector2Int.left;
            if(IsExist(map,curPos))
                Adjacencies.Add(map[curPos.x, curPos.y]);

            curPos = pos + Vector2Int.up;
            if(IsExist(map,curPos))
                Adjacencies.Add(map[curPos.x, curPos.y]);

            curPos = pos + Vector2Int.right;
            if(IsExist(map,curPos))
                Adjacencies.Add(map[curPos.x, curPos.y]);

            curPos = pos + Vector2Int.down;
            if(IsExist(map,curPos))
                Adjacencies.Add(map[curPos.x, curPos.y]);
            #endregion

            position = pos;


        }

        public void OnPointerClick(PointerEventData eventData) {
            onTileClick.Invoke(position);
        }

        private void OnDrawGizmos()
        {
            if(Adjacencies != null){
                for (int i = 0; i < Adjacencies.Count; i++){
                    Gizmos.DrawLine(transform.position, Adjacencies[i].transform.position);
                }
            }
        }

    }
}

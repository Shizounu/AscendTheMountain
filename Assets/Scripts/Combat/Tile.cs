using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Shizounu.Library.AI;
using System.Numerics;
namespace Combat {
    public class Tile : MonoBehaviour, IAStarTile
    {

        /// <summary>
        /// TODO make dependant on potential hazards reducing movespeed
        /// Make dependant on if it can be traveresed
        /// </summary>
        public float TraversalCost { get => 1; set => throw new System.NotImplementedException(); }
        public float Heuristic(IAStarTile goal)
        {
            return 1f;
        }
        public List<IAStarTile> Adjacencies { get; set; }

        public void Init(Vector2Int pos, Tile[,] map){
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
        }
    }
}

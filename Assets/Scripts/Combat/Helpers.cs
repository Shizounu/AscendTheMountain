using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public interface ICopyable<T>
    {
        public T GetCopy();
    }

    public static class BoardHelpers
    {
        public static Vector2Int[] Mask4 = new Vector2Int[] 
        {
            Vector2Int.left, 
            Vector2Int.right, 
            Vector2Int.up, 
            Vector2Int.down
        };
        public static Vector2Int[] Mask8 = new Vector2Int[] {
            Vector2Int.up + Vector2Int.left,
            Vector2Int.up,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down,
            Vector2Int.down + Vector2Int.right
        };
    }
}

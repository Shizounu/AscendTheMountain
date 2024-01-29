using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [System.Serializable]
    public class Tile : ICopyable<Tile>
    {
        public Tile(Vector2Int pos)
        {
            position = pos;
        }
        public Tile(Tile tileToCopy)
        {
            position = tileToCopy.position;
            if (tileToCopy.unit != null)
                unit = tileToCopy.unit.GetCopy();
        }
        public Vector2Int position;
        public Unit unit;



        public bool isFree => unit == null;

        public bool getIsPassable(Actors owner)
        {
            return unit == null || unit.owner == owner;

        }

        public Tile GetCopy() {
            return new Tile(this);
        }
    }
}
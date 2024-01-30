using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            unitID = tileToCopy.unitID;
        }
        public Vector2Int position;
        public string unitID;



        public bool isFree => unitID == "";

        public bool getIsPassable(Board board, Actors owner) {
            return unitID == "" || board.GetActorReference(owner).LivingUnitIDs.Any(unitRef => unitRef.unitID == unitID);
        }

        public Tile GetCopy() {
            return new Tile(this);
        }
    }
}
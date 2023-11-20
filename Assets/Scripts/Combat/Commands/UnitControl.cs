using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_SummonUnit : ICommand
    {
        public Command_SummonUnit()
        {

        }
        public Command_SummonUnit(Cards.UnitDefinition cardDefinition, Vector2Int _position, Actors _owner)
        {
            unitDef = cardDefinition;
            position = _position;
            owner = _owner;
        }

        [SerializeField] private Cards.UnitDefinition unitDef;
        [SerializeField] private Vector2Int position;
        [SerializeField] private Actors owner;

        public void Execute(Board board)
        {
            Unit unit = new Unit(unitDef, owner);
            board.tiles[position.x, position.y].unit = unit;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_RemoveUnit : ICommand
    {
        public Command_RemoveUnit()
        {

        }
        public Command_RemoveUnit(Unit _unit)
        {
            unit = _unit;
        }
        [SerializeField] private Unit unit;

        public void Execute(Board board)
        {
            for (int x = 0; x < board.tiles.GetLength(0); x++)
            {
                for (int y = 0; y < board.tiles.GetLength(1); y++)
                {
                    if (board.tiles[x, y].unit == unit)
                    {
                        board.tiles[x, y].unit = null;
                    }

                }
            }
            unit = null;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}
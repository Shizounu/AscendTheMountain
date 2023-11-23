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

    public class Command_MoveUnit : ICommand {
        public Command_MoveUnit()
        {
            
        }
        /// <summary>
        /// for moving one tile
        /// </summary>
        /// <param name="closePosition"></param>
        public Command_MoveUnit(Vector2Int startPos,Vector2Int closePosition) {
            this.startPos = startPos;
            path = new() { closePosition };
        }
        public Command_MoveUnit(Vector2Int startPos, List<Vector2Int> path)
        {
            this.startPos = startPos;
            this.path = new(path);
        }

        private Vector2Int startPos;
        private List<Vector2Int> path;

        public void Execute(Board board) {
            foreach (Vector2Int position in path){
                Move(board, position);
            }
        }
        private void Move(Board board, Vector2Int moveTo)
        {
            Unit unitRef = board.tiles[startPos.x, startPos.y].unit;
            board.tiles[startPos.x, startPos.y].unit = null;
            board.tiles[moveTo.x, moveTo.y].unit = unitRef;
            startPos = moveTo;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_DamageUnit : ICommand
    {
        public Command_DamageUnit() {
            
        }
        public Command_DamageUnit(int amount, Unit target) {
            this.amount = amount;
            this.target = target;
        }
        int amount;
        Unit target;

        public void Execute(Board board)
        {
            target.curHealth -= amount;
            if(target.curHealth <= 0) {
                board.SetSubCommand(new Command_KillUnit(target));
            }
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_KillUnit : ICommand {
        public Command_KillUnit(Unit unit)
        {
            this.unit = unit;
        }
        Unit unit;
        public void Execute(Board board)
        {
            //TODO Throw on death

            board.SetSubCommand(new Command_RemoveUnit(unit));
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_SetCanMove : ICommand {
        public Command_SetCanMove(){
            
        }
        public Command_SetCanMove(Unit unit, bool value)
        {
            this.unit = unit;
            this.value = value;
        }
        Unit unit;
        bool value;

        public void Execute(Board board)
        {
            unit.canMove = value;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
    public class Command_SetCanAttack : ICommand
    {
        public Command_SetCanAttack()
        {

        }
        public Command_SetCanAttack(Unit unit, bool value)
        {
            this.unit = unit;
            this.value = value;
        }
        Unit unit;
        bool value;

        public void Execute(Board board)
        {
            unit.canAttack = value;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}

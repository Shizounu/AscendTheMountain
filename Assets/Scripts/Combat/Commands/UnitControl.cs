using Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

namespace Commands
{
    public class Command_SummonUnit : Pool.Poolable<Command_SummonUnit>, ICommand, IVisualCommand {
        public Command_SummonUnit Init(Cards.UnitDefinition cardDefinition, Vector2Int _position, Actors _owner, bool canMove = false, bool canAttack = false, bool payCost = false, bool removeFromHand = false, int handIndex = 0)
        {
            unitDef = cardDefinition;
            position = _position;
            owner = _owner;

            this.canMove = canMove;
            this.canAttack = canAttack;
            this.payCost = payCost;
            this.removeFromHand = removeFromHand;
            this.handIndex = handIndex;

            return this;
        }


        Cards.UnitDefinition unitDef;
        Vector2Int position;
        public Actors owner;

        private bool canMove;
        private bool canAttack;
        private bool payCost;
        private bool removeFromHand;
        public int handIndex;


        Unit unit;
        public void Execute(Board board)
        {

            unit = new Unit(unitDef, owner);
            board.tiles[position.x, position.y].unit = unit;

            board.SetSubCommand(Command_SetCanMove.GetAvailable().Init(unit, canMove));
            board.SetSubCommand(Command_SetCanAttack.GetAvailable().Init(unit, canAttack));

            if (payCost)
                board.SetSubCommand(Command_ChangeCurrentMana.GetAvailable().Init(owner, -unitDef.Cost));
            if (removeFromHand)
                board.SetSubCommand(Command_RemoveHandCard.GetAvailable().Init(handIndex, owner));

            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.SpawnUnitVisuals(unit, unitDef.animatorController, position);
        }
    }
    public class Command_RemoveUnit : Pool.Poolable<Command_RemoveUnit>, ICommand, IVisualCommand
    {
        public Command_RemoveUnit Init(Unit _unit)
        {
            unit = _unit;
            return this;
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
            ReturnToPool(this);

        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.StartCoroutine(waitForAnimFinish(boardRenderer));
        }

        IEnumerator waitForAnimFinish(BoardRenderer boardRenderer)
        {
            //yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(boardRenderer.units[unit].getDeathAnimLength());
            BoardRenderer.Destroy(boardRenderer.units[unit].gameObject);
            boardRenderer.units.Remove(unit);
        }
    }
    public class Command_MoveUnit : Pool.Poolable<Command_MoveUnit>, ICommand, IVisualCommand {
        public Command_MoveUnit Init(Vector2Int startPos, Vector2Int goalPos) {
            this.startPos = startPos;
            this.goalPos = goalPos;
            return this;
        }


        public Vector2Int startPos;
        public Vector2Int curPos;
        public Vector2Int goalPos;

        Unit unitRef;
        public void Execute(Board board) {
            unitRef = board.GetUnitFromPos(startPos);
            curPos = startPos;
            
            Move(board, goalPos);

            board.SetSubCommand(Command_SetCanMove.GetAvailable().Init(unitRef, false));
            ReturnToPool(this);
        }
        private void Move(Board board, Vector2Int moveTo)
        {
            board.tiles[curPos.x, curPos.y].unit = null;
            board.tiles[moveTo.x, moveTo.y].unit = unitRef;
            curPos = moveTo;
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            UnitRenderer unitRenderer = boardRenderer.units[unitRef];
            unitRenderer.StartCoroutine(DoMoveStepsVisual(unitRenderer));
        }

        IEnumerator DoMoveStepsVisual(UnitRenderer unitRenderer) {
            unitRenderer.animRef = unitRenderer.StartCoroutine(unitRenderer.Move(goalPos));
            yield return new WaitUntil(() => unitRenderer.animRef == null);
        }
    }
    public class Command_AttackUnit : Pool.Poolable<Command_AttackUnit>, ICommand, IVisualCommand {
        public Command_AttackUnit Init(Unit attacker, Unit defender) {
            this.attacker = attacker;
            this.defender = defender;
            return this;
        }

        Unit attacker;
        Unit defender;

        public void Execute(Board board)
        {
            board.SetSubCommand(Command_DamageUnit.GetAvailable().Init(attacker.attack, defender));
            board.SetSubCommand(Command_SetCanAttack.GetAvailable().Init(attacker, false));
            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[attacker].OnAttack();
        }
    }
    public class Command_DamageUnit : Pool.Poolable<Command_DamageUnit>, ICommand, IVisualCommand
    {
        public Command_DamageUnit Init(int amount, Unit target) {
            this.amount = amount;
            this.target = target;
            return this;
        }
        int amount;
        Unit target;

        public void Execute(Board board)
        {
            target.curHealth -= amount;
            if(target.curHealth <= 0) {
                board.SetSubCommand(Command_KillUnit.GetAvailable().Init(target));
            }
            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[target].OnDamage();
        }
    }
    public class Command_KillUnit : Pool.Poolable<Command_KillUnit>, ICommand, IVisualCommand {
        public Command_KillUnit Init(Unit unit) {
            this.unit = unit;
            return this;
        }
        Unit unit;
        public void Execute(Board board)
        {
            //TODO Throw on death

            board.SetSubCommand(Command_RemoveUnit.GetAvailable().Init(unit));

            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[unit].OnDeath();
        }
    }
    public class Command_SetCanMove : Pool.Poolable<Command_SetCanMove>, ICommand {
        public Command_SetCanMove Init(Unit unit, bool value) {
            this.unit = unit;
            this.value = value;
            return this;
        }
        public Unit unit;
        public bool value;

        public void Execute(Board board)
        {
            unit.canMove = value;
            ReturnToPool(this);
        }
    }
    public class Command_SetCanAttack : Pool.Poolable<Command_SetCanAttack>,  ICommand
    {
        public Command_SetCanAttack Init(Unit unit, bool value) {
            this.unit = unit;
            this.value = value;
            return this;
        }
        Unit unit;
        bool value;

        public void Execute(Board board)
        {
            unit.canAttack = value;
            ReturnToPool(this);
        }
    }
}

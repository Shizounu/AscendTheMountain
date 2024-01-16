using Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_SummonUnit : ICommand, IVisualCommand
    {
        public Command_SummonUnit(Cards.UnitDefinition cardDefinition, Vector2Int _position, Actors _owner, bool canMove = false, bool canAttack = false, bool payCost = false, bool removeFromHand = false, int handIndex = 0)
        {
            unitDef = cardDefinition;
            position = _position;
            owner = _owner;

            this.canMove = canMove;
            this.canAttack = canAttack;
            this.payCost = payCost;
            this.removeFromHand = removeFromHand;
            this.handIndex = handIndex;
        }

        [SerializeField] public Cards.UnitDefinition unitDef;
        [SerializeField] public Vector2Int position;
        [SerializeField] public Actors owner;

        [SerializeField] private bool canMove;
        [SerializeField] private bool canAttack;
        [SerializeField] private bool payCost;
        [SerializeField] private bool removeFromHand;
        [SerializeField] public int handIndex;
        Unit unit;
        public void Execute(Board board)
        {
            unit = new Unit(unitDef, owner);
            board.tiles[position.x, position.y].unit = unit;

            board.SetSubCommand(new Command_SetCanMove(unit, canMove));
            board.SetSubCommand(new Command_SetCanAttack(unit, canAttack));

            if(payCost)
                board.SetSubCommand(new Command_SubCurrentMana(owner, unitDef.Cost));
            if (removeFromHand)
                board.SetSubCommand(new Command_RemoveHandCard(handIndex, owner));
            
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.SpawnUnitVisuals(unit, unitDef.animatorController, position);
        }
    }

    public class Command_RemoveUnit : ICommand, IVisualCommand
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


    //BUG WHEN PLAYER MOVES
    public class Command_MoveUnit : ICommand, IVisualCommand {
        /// <summary>
        /// for moving one tile
        /// </summary>
        /// <param name="closePosition"></param>
        public Command_MoveUnit(Vector2Int startPos, Vector2Int closePosition) {
            this.startPos = startPos;
            path = new() { closePosition };
        }
        public Command_MoveUnit(Vector2Int startPos, List<Vector2Int> path)
        {
            this.startPos = startPos;
            
            this.path = new(path);
        }
        public Vector2Int startPos;
        /// <summary>
        /// ADDED TO FIX NULL REF AND I DONT KNOW WHY IT FIXES IT AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        /// </summary>
        public Vector2Int curPos;
        public List<Vector2Int> path;

        Unit unitRef;
        public void Execute(Board board) {
            unitRef = board.GetUnitFromPos(startPos);
            curPos = startPos;
            foreach (Vector2Int position in path)
                Move(board, position);
            
            board.SetSubCommand(new Command_SetCanMove(unitRef, false));
        }
        private void Move(Board board, Vector2Int moveTo)
        {
            //save unit for moving
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
            for (int i = 0; i < path.Count; i++) {
                unitRenderer.animRef = unitRenderer.StartCoroutine(unitRenderer.Move(path[i]));
                yield return new WaitUntil(() => unitRenderer.animRef == null);
            }
        }
    }

    public class Command_AttackUnit : ICommand, IVisualCommand {
        public Command_AttackUnit()
        {
            
        }
        public Command_AttackUnit(Unit attacker, Unit defender)
        {
            this.attacker = attacker;
            this.defender = defender;
        }
        public Unit attacker;
        public Unit defender;

        public void Execute(Board board)
        {
            board.SetSubCommand(new Command_DamageUnit(attacker.attack, defender));
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[attacker].OnAttack();
        }
    }

    public class Command_DamageUnit : ICommand, IVisualCommand
    {
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

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[target].OnDamage();
        }
    }

    public class Command_KillUnit : ICommand, IVisualCommand {
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

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[unit].OnDeath();
        }
    }

    public class Command_SetCanMove : ICommand {
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
    }
}

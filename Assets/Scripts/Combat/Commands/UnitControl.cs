using Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
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

        public void InitForPooling(
            Cards.UnitDefinition unitDefinition, Vector2Int position, Actors owner,
            bool canMove, bool canAttack, bool payCost, bool removeFromHand, int handIndex,
            Command_SetCanMove Com_canMove, Command_SetCanAttack Com_canAttack, Command_SubCurrentMana Com_curMana, Command_RemoveHandCard Com_removeHandCard) {

            pooling = true;
            
            this.unitDef = unitDefinition;
            this.position = position;
            this.owner = owner;

            this.canMove = canMove;
            this.canAttack = canAttack;
            this.payCost = payCost;
            this.removeFromHand = removeFromHand;

            this.Com_canAttack = Com_canAttack;
            this.Com_canMove = Com_canMove;
            this.Com_subCurrentMana = Com_curMana;
            this.Com_removeHandCard = Com_removeHandCard;
        }
        bool pooling = false;


        [SerializeField] public Cards.UnitDefinition unitDef;
        [SerializeField] public Vector2Int position;
        [SerializeField] public Actors owner;

        [SerializeField] private bool canMove;
        [SerializeField] private bool canAttack;
        [SerializeField] private bool payCost;
        [SerializeField] private bool removeFromHand;
        [SerializeField] public int handIndex;

        Command_SetCanMove Com_canMove;
        Command_SetCanAttack Com_canAttack;
        Command_RemoveHandCard Com_removeHandCard;
        Command_SubCurrentMana Com_subCurrentMana;



        Unit unit;
        public void Execute(Board board)
        {
            if (!pooling) {
                unit = new Unit(unitDef, owner);
                board.tiles[position.x, position.y].unit = unit;

                board.SetSubCommand(new Command_SetCanMove(unit, canMove));
                board.SetSubCommand(new Command_SetCanAttack(unit, canAttack));

                if(payCost)
                    board.SetSubCommand(new Command_SubCurrentMana(owner, unitDef.Cost));
                if (removeFromHand)
                    board.SetSubCommand(new Command_RemoveHandCard(handIndex, owner));
            } else {
                unit = new Unit(unitDef, owner);
                board.tiles[position.x, position.y].unit = unit;

                Com_canMove.unit = unit;
                Com_canMove.value = canMove;
                board.SetSubCommand(Com_canMove);

                Com_canAttack.unit = unit;
                Com_canAttack.value = canAttack;
                board.SetSubCommand(Com_canAttack);

                if (removeFromHand) {
                    Com_removeHandCard.actor = owner;
                    Com_removeHandCard.handIndex = handIndex;
                    board.SetSubCommand(Com_removeHandCard);
                }
                if (payCost) {
                    Com_subCurrentMana.side = owner;
                    Com_subCurrentMana.amount = unitDef.Cost;
                    board.SetSubCommand(Com_subCurrentMana);
                }


            }
            
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

    public class Command_MoveUnit : ICommand, IVisualCommand {
        public Command_MoveUnit(Vector2Int startPos, Vector2Int closePosition) {
            this.startPos = startPos;
            path = new() { closePosition };
        }
        public Command_MoveUnit(Vector2Int startPos, List<Vector2Int> path)
        {
            this.startPos = startPos;
            
            this.path = new(path);
        }

        public void InitForPooling(Vector2Int startPos, Vector2Int closePosition, Command_SetCanMove Com_canMove)
        {
            pooling = true;
            this.startPos = startPos;
            this.path = new() { closePosition };
            CanMoveCommand = Com_canMove;
        }

        bool pooling = false;
        Command_SetCanMove CanMoveCommand;

        public Vector2Int startPos;
        /// <summary>
        /// ADDED TO FIX NULL REF AND I DONT KNOW WHY IT FIXES IT AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
        /// </summary>
        public Vector2Int curPos;
        public List<Vector2Int> path;

        Unit unitRef;
        public void Execute(Board board) {
            if (pooling) {
                unitRef = board.GetUnitFromPos(startPos);
                curPos = startPos;
                foreach (Vector2Int position in path)
                    Move(board, position);
            
                board.SetSubCommand(new Command_SetCanMove(unitRef, false));
            } else {
                unitRef = board.GetUnitFromPos(startPos);
                curPos = startPos;
                foreach (Vector2Int position in path)
                    Move(board, position);

                CanMoveCommand.unit = unitRef;
                CanMoveCommand.value = false;
                board.SetSubCommand(CanMoveCommand);
            }
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

        public void InitForPooling(Unit attacker, Unit defender, Command_SetCanAttack canAttack)
        {
            pooling = true;
            this.attacker = attacker;
            this.defender = defender;

            this.setCanAttack = canAttack;
        }
        bool pooling;
        Command_SetCanAttack setCanAttack;

        public Unit attacker;
        public Unit defender;

        public void Execute(Board board)
        {
            if (pooling) {
                board.SetSubCommand(new Command_DamageUnit(attacker.attack, defender));
                board.SetSubCommand(new Command_SetCanAttack(attacker, false)); 
            } else {
                board.SetSubCommand(new Command_DamageUnit(attacker.attack, defender));

                setCanAttack.value = false; 
                setCanAttack.unit = attacker;
                board.SetSubCommand(setCanAttack);
            }

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
        public Unit unit;
        public bool value;

        public void Execute(Board board)
        {
            unit.canMove = value;
        }
    }
    public class Command_SetCanAttack : ICommand
    {
        public Command_SetCanAttack(Unit unit, bool value)
        {
            this.unit = unit;
            this.value = value;
        }
        public Unit unit;
        public bool value;

        public void Execute(Board board)
        {
            unit.canAttack = value;
        }
    }
}

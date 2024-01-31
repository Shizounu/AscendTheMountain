using Combat;
using Combat.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_SummonGeneral : Pool.Poolable<Command_SummonGeneral>, ICommand, IVisualCommand {
        public Command_SummonGeneral Init(CardInstance_Unit generalCard, Vector2Int position, Actors owner) {
            this.generalCard = generalCard;
            this.position = position;
            this.owner = owner;

            return this; 
        }

        public CardInstance_Unit generalCard;
        public Vector2Int position;
        public Actors owner;

        private Unit general;

        public void Execute(Board board) {
            string ID = board.GetUID();
            general = new Unit(generalCard, owner, ID);
            board.GetActorReference(owner).GeneralReference = new UnitReference(ID, general, position);
            board.tiles[position.x, position.y].unitID = ID;



            board.SetSubCommand(Command_SetCanMove.GetAvailable().Init(ID, true));
            board.SetSubCommand(Command_SetCanAttack.GetAvailable().Init(ID, true));

            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.SpawnUnitVisuals(general, generalCard.cardAnimator, position);

        }
    }
    public class Command_SummonUnit : Pool.Poolable<Command_SummonUnit>, ICommand, IVisualCommand {
        public Command_SummonUnit Init(CardInstance_Unit cardDefinition, Vector2Int _position, Actors _owner, bool canMove = false, bool canAttack = false, bool payCost = false, bool removeFromHand = false, int handIndex = 0)
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


        CardInstance_Unit unitDef;
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
            string ID = board.GetUID();

            unit = new Unit(unitDef, owner, ID);
            board.tiles[position.x, position.y].unitID = ID;
            board.GetActorReference(owner).AddLivingUnitReference(ID, unit, position);


            board.SetSubCommand(Command_SetCanMove.GetAvailable().Init(ID, canMove));
            board.SetSubCommand(Command_SetCanAttack.GetAvailable().Init(ID, canAttack));

            if (payCost)
                board.SetSubCommand(Command_ChangeCurrentMana.GetAvailable().Init(owner, -unitDef.cardCost));
            if (removeFromHand)
                board.SetSubCommand(Command_RemoveHandCard.GetAvailable().Init(handIndex, owner));

            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.SpawnUnitVisuals(unit, unitDef.cardAnimator, position);
        }
    }
    public class Command_RemoveUnit : Pool.Poolable<Command_RemoveUnit>, ICommand, IVisualCommand
    {
        public Command_RemoveUnit Init(string unitID)
        {
            this.unitID = unitID;
            return this;
        }

        string unitID;
        public void Execute(Board board)
        {
            UnitReference unitRef = board.GetUnitReference(unitID);
            board.tiles[unitRef.unitPosition.x, unitRef.unitPosition.y].unitID = "";
            unitRef.unitReference = null;
            board.GetActorReference(unitRef.unitReference.owner).GetLivingUnits().Remove(unitRef);

            ReturnToPool(this);

        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.StartCoroutine(waitForAnimFinish(boardRenderer));
        }

        IEnumerator waitForAnimFinish(BoardRenderer boardRenderer)
        {
            //yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(boardRenderer.units[unitID].getDeathAnimLength());
            BoardRenderer.Destroy(boardRenderer.units[unitID].gameObject);
            boardRenderer.units.Remove(unitID);
        }
    }
    public class Command_MoveUnit : Pool.Poolable<Command_MoveUnit>, ICommand, IVisualCommand {
        public Command_MoveUnit Init(Vector2Int startPos, Vector2Int goalPos) {
            this.startPos = startPos;
            this.goalPos = goalPos;
            return this;
        }


        public Vector2Int startPos;
        private Vector2Int curPos;
        public Vector2Int goalPos;

        string unitID;
        public void Execute(Board board) {
            unitID = board.GetUnitReference(startPos).unitID;
            curPos = startPos;
            
            Move(board, goalPos);

            board.SetSubCommand(Command_SetCanMove.GetAvailable().Init(unitID, false));
            ReturnToPool(this);
        }
        private void Move(Board board, Vector2Int moveTo)
        {
            board.tiles[curPos.x, curPos.y].unitID = "";
            board.tiles[moveTo.x, moveTo.y].unitID = unitID;
            curPos = moveTo;
            board.GetUnitReference(unitID).ChangePosition(curPos);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            UnitRenderer unitRenderer = boardRenderer.units[unitID];
            unitRenderer.StartCoroutine(DoMoveStepsVisual(unitRenderer));
        }

        IEnumerator DoMoveStepsVisual(UnitRenderer unitRenderer) {
            unitRenderer.animRef = unitRenderer.StartCoroutine(unitRenderer.Move(goalPos));
            yield return new WaitUntil(() => unitRenderer.animRef == null);
        }
    }
    public class Command_AttackUnit : Pool.Poolable<Command_AttackUnit>, ICommand, IVisualCommand {
        public Command_AttackUnit Init(string attackerID, string defenderID) {
            this.attackerID = attackerID;
            this.defenderID = defenderID;
            return this;
        }

        string attackerID;
        string defenderID;

        public void Execute(Board board)
        {
            board.SetSubCommand(Command_DamageUnit.GetAvailable().Init(board.GetUnitReference(attackerID).unitReference.attack, defenderID));
            board.SetSubCommand(Command_SetCanAttack.GetAvailable().Init(attackerID, false));
            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[attackerID].OnAttack();
        }
    }
    public class Command_DamageUnit : Pool.Poolable<Command_DamageUnit>, ICommand, IVisualCommand {
        public Command_DamageUnit Init(int amount, string targetID) {
            this.amount = amount;
            this.targetID = targetID;
            return this;
        }
        int amount;
        string targetID;

        public void Execute(Board board)
        {
            board.GetUnitReference(targetID).unitReference.curHealth -= amount;
            if(board.GetUnitReference(targetID).unitReference.curHealth <= 0) {
                board.SetSubCommand(Command_KillUnit.GetAvailable().Init(targetID));
            }
            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[targetID].OnDamage();
        }
    }
    public class Command_KillUnit : Pool.Poolable<Command_KillUnit>, ICommand, IVisualCommand {
        public Command_KillUnit Init(string unitID) {
            this.unitID = unitID;
            return this;
        }
        string unitID;
        public void Execute(Board board)
        {
            board.SetSubCommand(Command_RemoveUnit.GetAvailable().Init(unitID));

            ReturnToPool(this);
        }

        public void Visuals(BoardRenderer boardRenderer)
        {
            boardRenderer.units[unitID].OnDeath();
        }
    }
    public class Command_SetCanMove : Pool.Poolable<Command_SetCanMove>, ICommand {
        public Command_SetCanMove Init(string unitID, bool value) {
            this.unitID = unitID;
            this.value = value;
            return this;
        }

        public string unitID;
        public bool value;

        public void Execute(Board board) {
            board.GetUnitReference(this.unitID).unitReference.canMove = value;
            ReturnToPool(this);
        }
    }
    public class Command_SetCanAttack : Pool.Poolable<Command_SetCanAttack>,  ICommand
    {
        public Command_SetCanAttack Init(string unitID, bool value) {
            this.unitID = unitID;
            this.value = value;
            return this;
        }
        string unitID;
        bool value;

        public void Execute(Board board)
        {
            board.GetUnitReference(this.unitID).unitReference.canAttack = value;
            ReturnToPool(this);
        }
    }
}

using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_EndTurn : Pool.Poolable<Command_EndTurn>, ICommand {
        public Command_EndTurn Init(Actors disabledSide) {
            this.side = disabledSide;
            return this;
        }
        Actors side;

        public void Execute(Board board)
        {
            board.SetSubCommand(Command_OnTurnStart.GetAvailable().Init(side));
            if(side == Actors.Actor1) {
                board.SetSubCommand(Command_SetEnable.GetAvailable().Init(Actors.Actor1, true));
                board.SetSubCommand(Command_SetEnable.GetAvailable().Init(Actors.Actor2, false));
            } else {
                board.SetSubCommand(Command_SetEnable.GetAvailable().Init(Actors.Actor1, false));
                board.SetSubCommand(Command_SetEnable.GetAvailable().Init(Actors.Actor2, true));
            }
            ReturnToPool(this);
        }
    }
    
    public class Command_OnTurnStart : Pool.Poolable<Command_OnTurnStart>, ICommand{ 
        public Command_OnTurnStart Init(Actors side) {
            this.Side = side;
            return this;
        }
        Actors Side;

        public void Execute(Board board)
        {
            foreach (var unit in board.GetActorReference(Side).GetLivingUnits()) {
                board.SetSubCommand(Command_SetCanMove.GetAvailable().Init(unit.unitID, true));
                board.SetSubCommand(Command_SetCanAttack.GetAvailable().Init(unit.unitID, true));
            }

            board.SetSubCommand(Command_ChangeMaxMana.GetAvailable().Init(Side, 1));
            board.SetSubCommand(Command_ChangeCurrentMana.GetAvailable().Init(Side, 69)); //Number chosen to be a high number that would fill the entire mana bar, gets culled down to Maxmana during set

            ReturnToPool(this);
        }
    }

    public class Command_SetEnable : Pool.Poolable<Command_SetEnable>, ICommand {
        public Command_SetEnable Init(Actors side, bool val)
        {
            this.side = side;
            this.val = val;
            return this;
        }
        public Actors side;
        public bool val;

        public void Execute(Board board)
        {
            //Gets triggered in game maanger, I also dont know why I did it that way
            ReturnToPool(this);
        }
    }
}
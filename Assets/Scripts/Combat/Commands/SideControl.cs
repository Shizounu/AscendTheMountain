using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_EnableSide : Pool.Poolable<Command_EnableSide>, ICommand {
        public Command_EnableSide Init(Actors disabledSide) {
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

        Tile[,] Tiles => GameManager.Instance.currentBoard.tiles;

        public void Execute(Board board)
        {


            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    if (Tiles[x, y].unit?.owner == Side)
                    {
                        board.SetSubCommand(Command_SetCanMove.GetAvailable().Init(Tiles[x, y].unit, true));
                        board.SetSubCommand(Command_SetCanAttack.GetAvailable().Init(Tiles[x, y].unit, true));
                        //TODO: Add on turn start effect triggering
                    }

                }
            }
            board.SetSubCommand(Command_ChangeCurrentMana.GetAvailable().Init(Side, 1));
            board.SetSubCommand(Command_ChangeCurrentMana.GetAvailable().Init(Side, 69));

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
        Actors side;
        bool val;

        public void Execute(Board board)
        {
            if(val)
                board.getActorReference(side).Enable();
            else 
                board.getActorReference(side).Disable();

            ReturnToPool(this);
        }
    }
}
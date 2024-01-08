using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_EnableSide : ICommand {
        /// <summary>
        /// Argument is the side that is to be disabled
        /// </summary>
        /// <param name="disabledSide"></param>
        public Command_EnableSide(Actors disabledSide) {
            this.side = disabledSide;
        }
        Actors side;

        public void Execute(Board board)
        {
            if(side == Actors.Actor1) {
                board.SetSubCommand(new Command_OnTurnStart(side));
                board.SetSubCommand(new Command_SetEnable(Actors.Actor1, true));
                board.SetSubCommand(new Command_SetEnable(Actors.Actor2, false));
            } else {
                board.SetSubCommand(new Command_OnTurnStart(side));
                board.SetSubCommand(new Command_SetEnable(Actors.Actor1, true));
                board.SetSubCommand(new Command_SetEnable(Actors.Actor2, true));

            }
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
    

    public class Command_OnTurnStart : ICommand{ 
        public Command_OnTurnStart(Actors side) {
            this.Side = side;
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
                        board.SetSubCommand(new Command_SetCanMove(Tiles[x, y].unit, true));
                        board.SetSubCommand(new Command_SetCanAttack(Tiles[x, y].unit, true));
                        //TODO: Add on turn start effect triggering
                    }

                }
            }
            board.SetSubCommand(new Command_AddMaxMana(Actors.Actor2, 1));
            board.SetSubCommand(new Command_AddCurrentMana(Actors.Actor2, 69));
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_SetEnable : ICommand {
        public Command_SetEnable(Actors side, bool val)
        {
            this.side = side;
            this.val = val;
        }
        Actors side;
        bool val;

        public void Execute(Board board)
        {
            if(val)
                board.getActorReference(side).Enable();
            else 
                board.getActorReference(side).Disable();
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}
using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_SwitchSide : ICommand {
        public Command_SwitchSide() { }
        /// <summary>
        /// Argument is the side that is to be disabled
        /// </summary>
        /// <param name="disabledSide"></param>
        public Command_SwitchSide(Actors disabledSide) {
            this.side = disabledSide;
        }
        Actors side;

        public void Execute(Board board)
        {
            if(side == Actors.Actor1) {
                board.Actor1_Deck.Disable();
                board.Actor2_Deck.Enable();

                board.SetSubCommand(
                    new Command_AddMaxMana(Actors.Actor2, 1)
                    );
                board.SetSubCommand(
                    new Command_AddCurrentMana(Actors.Actor2, board.Actor2_Deck.MaxManagems)
                    );



            } else {
                board.Actor1_Deck.Enable();
                board.Actor2_Deck.Disable();

                board.SetSubCommand(
                    new Command_AddMaxMana(Actors.Actor1, 1)
                );
                board.SetSubCommand(
                    new Command_AddCurrentMana(Actors.Actor1, 10)
                );
            }
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_OnTurnStart : ICommand{
        public Command_OnTurnStart()
        {
            
        }
        public Command_OnTurnStart(Actors side) {
            this.Side = side;
        }
        Actors Side;

        Tile[,] Tiles => GameManager.Instance.currentBoard.tiles;

        public void Execute(Board board)
        {
            for (int x = 0; x < Tiles.GetLength(0); x++) {
                for (int y = 0; y < Tiles.GetLength(1); y++) {
                    if (Tiles[x,y].unit?.owner == Side) {
                        board.SetSubCommand(new Command_SetCanMove(Tiles[x, y].unit, true));
                        board.SetSubCommand(new Command_SetCanAttack(Tiles[x, y].unit, true));
                        //TODO: Add on turn start effect triggering
                    }
                
                }
            }
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}
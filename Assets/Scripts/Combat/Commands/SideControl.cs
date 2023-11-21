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
}
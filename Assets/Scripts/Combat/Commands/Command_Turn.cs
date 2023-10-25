using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_IncrementTurn : ICommand
    {
        public void Execute(Board board)
        {
            board.currentActor += 1;
            if(board.currentActor == Actors.Reset)
                board.currentActor = 0;

            board.SetSubCommand(new Command_DoStartPhase());
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_SetTurn : ICommand {
        public Command_SetTurn(Actors _actor) {
            actor = _actor;
        }
        public Actors actor;
        public void Execute(Board board)
        {
            board.currentActor = actor;
            board.SetSubCommand(new Command_DoStartPhase());
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

}

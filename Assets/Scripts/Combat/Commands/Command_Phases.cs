using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Commands
{
    public class Command_DoStartPhase : ICommand {
        public void Execute(Board board) {
            board.currentPhase = Phase.StartPhase;
            for (int x = 0; x < board.tiles.GetLength(0); x++) {
                for (int y = 0; y < board.tiles.GetLength(1); y++) {
                    //Try get command, if its null skip it. ? for skipping if there is no player
                    foreach (var effect in board.tiles[x, y].unit?.effect) {
                        ICommand command = Helpers.GetInterface<ITrigger_OnPhaseStart>(effect);
                        if (command != null)
                            board.SetSubCommand(command);
                    }
                }
            }
            board.SetSubCommand(new Command_DoPhase());
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_DoPhase : ICommand
    {
        public void Execute(Board board)
        {
            board.currentPhase = Phase.Phase;
            //Enable current sides control
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_DoEndPhase : ICommand
    {
        public void Execute(Board board)
        {
            board.currentPhase = Phase.EndPhase;
            for (int x = 0; x < board.tiles.GetLength(0); x++)
            {
                for (int y = 0; y < board.tiles.GetLength(1); y++)
                {
                    //Try get command, if its null skip it. ? for skipping if there is no player
                    foreach (var effect in board.tiles[x, y].unit?.effect){
                        ICommand command = Helpers.GetInterface<ITrigger_OnPhaseEnd>(effect);
                        if (command != null)
                            board.SetSubCommand(command);
                    }
                    
                }
            }
            board.SetSubCommand(new Command_IncrementTurn());
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}

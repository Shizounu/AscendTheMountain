using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shizounu.Library.AI;


namespace Combat
{
    public enum Actor
    {
        Actor1,
        Actor2,
        Reset
    }
    public enum Phase
    {
        StartPhase,
        Phase,
        EndPhase
    }

    public class Board {
        public Board() {
            tiles = new Tile[5, 9];
        }
        public Board(Vector2Int boardSize) {
            tiles = new Tile[boardSize.x, boardSize.y];
        }

        public Tile[,] tiles;
        public Actor currentActor;
        public Phase currentPhase;
        


        private Queue<ICommand> commandQueue = new Queue<ICommand>();
        /// <summary>
        /// Sub command queue is for commands that get executed my other commands and need to happen right after them
        /// Not manually added to, only by other commands. All commands added by a command go herek
        /// </summary>
        private Queue<ICommand> subCommandQueue = new Queue<ICommand>();
        public void SetCommand(ICommand command) {
            commandQueue.Enqueue(command);  
        }
        public void SetSubCommand(ICommand subCommand) { 
            subCommandQueue.Enqueue(subCommand);
        }

        public void DoQueuedCommands() {
            do
            {
                do
                {
                    subCommandQueue.Dequeue().Execute(this);
                } while (subCommandQueue.Count > 0);
                commandQueue.Dequeue().Execute(this);
            } while (commandQueue.Count > 0);
        }

       


    }

    public class Tile {
        public Vector2Int position;
        public Unit unit;
        public Obstacle obstacle;
    }
}

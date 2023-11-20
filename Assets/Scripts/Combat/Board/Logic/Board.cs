using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shizounu.Library.AI;

using Commands;
namespace Combat
{


    [System.Serializable]
    public class Board {
        public Board() {
            tiles = new Tile[9, 5];

            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    tiles[x, y] = new(new Vector2Int(x,y));
                }
            }
        }
        public Board(OnCommandHandler onCommand) {
            this.onCommand = onCommand;
        }
        OnCommandHandler onCommand;

        public Board(Vector2Int boardSize) {
            tiles = new Tile[boardSize.x, boardSize.y];


        }
        /// <summary>
        /// Holds the information about each of the tiles
        /// </summary>
        public Tile[,] tiles;

        public DeckInformation Actor1_Deck;
        public DeckInformation Actor2_Deck;


        #region Command Handling
        private Queue<ICommand> commandQueue = new Queue<ICommand>();
        /// <summary>
        /// Sub command queue is for commands that get executed my other commands and need to happen right after them
        /// Not manually added to, only by other commands. All commands added by a command go herek
        /// </summary>
        private Queue<ICommand> subCommandQueue = new Queue<ICommand>();
        public void SetCommand(ICommand command)
        {
            commandQueue.Enqueue(command);
        }
        public void SetSubCommand(ICommand subCommand)
        {
            subCommandQueue.Enqueue(subCommand);
        }

        public void DoQueuedCommands()
        {
            while (commandQueue.Count > 0)
            {
                ICommand curCommand = commandQueue.Dequeue();
                curCommand.Execute(this);
                onCommand?.Invoke(curCommand);

                while (subCommandQueue.Count > 0) {
                    curCommand = subCommandQueue.Dequeue();
                    curCommand.Execute(this);
                    onCommand?.Invoke(curCommand);
                }
            }
        }
        #endregion
    
        public DeckInformation getActorReference(Actors actors) {
            return actors == Actors.Actor1 ? Actor1_Deck : Actor2_Deck;
        }
    }

    public enum Actors
    {
        /// <summary>
        /// Defaulted to Player
        /// </summary>
        Actor1,
        /// <summary>
        /// Defaulted to AI
        /// </summary>
        Actor2
    }
    

    [System.Serializable]
    public class Tile {
        public Tile(Vector2Int pos) {
            position = pos;
        }
        public Vector2Int position;
        public Unit unit;
        public Obstacle obstacle;
    }
}

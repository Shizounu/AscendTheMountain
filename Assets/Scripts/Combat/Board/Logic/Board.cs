using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shizounu.Library.AI;

using Commands;
using JetBrains.Annotations;
using UnityEngine.InputSystem.LowLevel;

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

            Actor1_Deck = new();
            Actor2_Deck = new();
        }
        public OnCommandHandler onCommand;

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

        public List<Vector2Int> getMovePositions(Vector2Int unitPos, int moveDist){
            bool isInBounds(Vector2Int pos) {
                return pos.x < tiles.GetLength(0) && pos.x >= 0 &&
                       pos.y < tiles.GetLength(1) && pos.y >= 0;
            }
            bool isStandable(Tile tile) {
                return tile.unit == null;
            }
            bool isPassable(Tile tile) {
                return tile.unit == null || tile.unit.owner == tiles[unitPos.x, unitPos.y].unit.owner;
            }

            List<Vector2Int> getPositions(Vector2Int curPos, int remainDist ) {
                List<Vector2Int> result = new();
                if (isStandable(tiles[curPos.x, curPos.y]))
                    result.Add(curPos);
                Vector2Int nextPos;

                nextPos = curPos + Vector2Int.left;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (isPassable(tiles[nextPos.x, nextPos.y]) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));

                nextPos = curPos + Vector2Int.right;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (isPassable(tiles[nextPos.x, nextPos.y]) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));

                
                nextPos = curPos + Vector2Int.up;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (isPassable(tiles[nextPos.x, nextPos.y]) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));

          
                nextPos = curPos + Vector2Int.down;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (isPassable(tiles[nextPos.x, nextPos.y]) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));
                
                return result;
            }

            return getPositions(unitPos, moveDist);
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

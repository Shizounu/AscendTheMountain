using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shizounu.Library.AI;

using Commands;
using JetBrains.Annotations;
using UnityEngine.InputSystem.LowLevel;
using Unity.VisualScripting;
using System;
using System.Security.Cryptography;
using System.Text;

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

        public Board(Board boardToCopy)
        {
            tiles = new Tile[9, 5];

            for (int x = 0; x < tiles.GetLength(0); x++)
                for (int y = 0; y < tiles.GetLength(1); y++)
                    tiles[x, y] = new Tile(boardToCopy.tiles[x,y]);

            Actor1_Deck = boardToCopy.Actor1_Deck.Clone();
            Actor2_Deck = boardToCopy.Actor2_Deck.Clone();
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

        #region Board Info Methods
        public DeckInformation getActorReference(Actors actors) {
            return actors == Actors.Actor1 ? Actor1_Deck : Actor2_Deck;
        }
        public List<Vector2Int> getMovePositions(Vector2Int unitPos, int moveDist){
            List<Vector2Int> getPositions(Vector2Int curPos, int remainDist ) {
                List<Vector2Int> result = new();
                if (tiles[curPos.x, curPos.y].isFree)
                    result.Add(curPos);
                Vector2Int nextPos;

                nextPos = curPos + Vector2Int.left;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (tiles[nextPos.x, nextPos.y].getIsPassable(GetUnitFromPos(unitPos).owner) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));

                nextPos = curPos + Vector2Int.right;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (tiles[nextPos.x, nextPos.y].getIsPassable(GetUnitFromPos(unitPos).owner) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));

                
                nextPos = curPos + Vector2Int.up;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (tiles[nextPos.x, nextPos.y].getIsPassable(GetUnitFromPos(unitPos).owner) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));

          
                nextPos = curPos + Vector2Int.down;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (tiles[nextPos.x, nextPos.y].getIsPassable(GetUnitFromPos(unitPos).owner) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));
                
                return result;
            }

            return getPositions(unitPos, moveDist);
        }
        public List<Vector2Int> getMovePositions(Vector2Int unitPos)
        {
            List<Vector2Int> getPositions(Vector2Int curPos, int remainDist)
            {
                List<Vector2Int> result = new();
                if (tiles[curPos.x, curPos.y].isFree)
                    result.Add(curPos);
                Vector2Int nextPos;

                nextPos = curPos + Vector2Int.left;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (tiles[nextPos.x, nextPos.y].getIsPassable(GetUnitFromPos(unitPos).owner) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));

                nextPos = curPos + Vector2Int.right;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (tiles[nextPos.x, nextPos.y].getIsPassable(GetUnitFromPos(unitPos).owner) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));


                nextPos = curPos + Vector2Int.up;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (tiles[nextPos.x, nextPos.y].getIsPassable(GetUnitFromPos(unitPos).owner) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));


                nextPos = curPos + Vector2Int.down;
                if (isInBounds(nextPos) && !result.Contains(nextPos))
                    if (tiles[nextPos.x, nextPos.y].getIsPassable(GetUnitFromPos(unitPos).owner) && remainDist > 0)
                        result.AddRange(getPositions(nextPos, remainDist - 1));

                return result;
            }

            return getPositions(unitPos, GetUnitFromPos(unitPos).moveDistance);
        }
        public List<Vector2Int> getAttackPositions(Vector2Int position) {

            List<Vector2Int> result = new();

            if (isInBounds(position + Vector2Int.up))
                result.Add(position + Vector2Int.up);
            if (isInBounds(position + Vector2Int.down))
                result.Add(position + Vector2Int.down);
            if (isInBounds(position + Vector2Int.right))
                result.Add(position + Vector2Int.right);
            if (isInBounds(position + Vector2Int.left))
                result.Add(position + Vector2Int.left);

            return result;
        }
        public List<Vector2Int> getSummonPositions(Actors actor)
        {
            List<Vector2Int> unitPositions = new();
            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    if (GetUnitFromPos(x, y)?.owner == actor)
                        unitPositions.Add(new Vector2Int(x, y));
                }
            }
            List<Vector2Int> result = new();   

            for (int i = 0; i < unitPositions.Count; i++) {
                List<Vector2Int> surroundingTiles = new List<Vector2Int>() {
                    Vector2Int.up + Vector2Int.left,
                    Vector2Int.up, 
                    Vector2Int.up + Vector2Int.right,
                    Vector2Int.left, 
                    Vector2Int.right, 
                    Vector2Int.down + Vector2Int.left,
                    Vector2Int.down, 
                    Vector2Int.down + Vector2Int.right
                };

                for (int j = 0; j < surroundingTiles.Count; j++) {
                    Vector2Int pos = unitPositions[i] + surroundingTiles[j];
                    if (isInBounds(pos)) {
                        if (tiles[pos.x, pos.y].isFree)
                            result.Add(pos);
                    }
                }


            }
            return result;
        }
        public List<Vector2Int> GetUnitPositions(Actors owner) {
            List<Vector2Int> unitPositions = new List<Vector2Int>();
            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    if(GetUnitFromPos(x,y) != null)
                        if(GetUnitFromPos(x,y).owner == owner)
                            unitPositions.Add(new Vector2Int(x, y));
                }
            }
            return unitPositions;
        }
        #endregion
        
        #region Helpers
        public Unit GetUnitFromPos(Vector2Int pos) {
            return tiles[pos.x, pos.y].unit;
        }
        public Unit GetUnitFromPos(int x, int y)
        {
            return tiles[x, y].unit;
        }
        public bool isInBounds(Vector2Int pos)
        {
            return pos.x < tiles.GetLength(0) && pos.x >= 0 &&
                   pos.y < tiles.GetLength(1) && pos.y >= 0;
        }
        public string GetJSON()
        {
            Board copy = new Board(this);
            return JsonUtility.ToJson(copy, true);
        }
        public string GetHash() {
            string JSON = GetJSON();

            // https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp

            byte[] buffer;
            using (HashAlgorithm algorithm = SHA256.Create())
                buffer = algorithm.ComputeHash(Encoding.UTF8.GetBytes(JSON));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in buffer)
                sb.Append(b.ToString("X2"));

            return sb.ToString();

        }
        #endregion


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
        public Tile(Tile tileToCopy) {
            position = tileToCopy.position;
            if(tileToCopy.unit != null)
                unit = new Unit(tileToCopy.unit);
        }
        public Vector2Int position;
        public Unit unit;

        public bool isFree => unit == null;

        public bool getIsPassable(Actors owner)
        {
            return unit == null || unit.owner == owner;
            
        }
    }
}

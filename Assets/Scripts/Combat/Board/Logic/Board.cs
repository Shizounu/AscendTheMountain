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
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;
using System.Linq;

namespace Combat
{


    [Serializable]
    public class Board : ICopyable<Board> {
        public Board() {
            tiles = new Tile[9, 5];

            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    tiles[x, y] = new(new Vector2Int(x,y));
                }
            }

            Actor1_Deck = new();
            Actor2_Deck = new();

            currentUnitIndex = 0;
        }

        private Board(Board baseBoard)
        {
            tiles = new Tile[9, 5];

            for (int x = 0; x < tiles.GetLength(0); x++)
                for (int y = 0; y < tiles.GetLength(1); y++)
                    tiles[x, y] = new Tile(baseBoard.tiles[x,y]);
            
            Actor1_Deck = baseBoard.Actor1_Deck.GetCopy();
            Actor2_Deck = baseBoard.Actor2_Deck.GetCopy();

            this.currentUnitIndex = baseBoard.currentUnitIndex;
        }
        
        public event OnCommandHandler onCommand;

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
        /// Not manually added to, only by other commands. All commands added by a command go here
        /// </summary>
        private Queue<ICommand> subCommandQueue = new Queue<ICommand>();
        
        public int curCommandCount => commandQueue.Count + subCommandQueue.Count;
        
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
            while (commandQueue.Count > 0) {
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
        public DeckInformation GetActorReference(Actors actors) {
            return actors == Actors.Actor1 ? Actor1_Deck : Actor2_Deck;
        }
        public List<Vector2Int> GetMovePositions(Vector2Int basePos, Actors owner, int moveDist) {
            List<Vector2Int> result = new();
            if (tiles[basePos.x, basePos.y].isFree)
                result.Add(basePos);

            if (moveDist <= 0)
                return result;

            for (int i = 0; i < BoardHelpers.Mask4.Length; i++) {
                Vector2Int curPos = basePos + BoardHelpers.Mask4[i];
                if (IsInBounds(curPos) && !result.Contains(curPos) && tiles[curPos.x, curPos.y].getIsPassable(this,owner)) {
                    result.AddRange(GetMovePositions(curPos, owner,moveDist - 1));
                }
            }

            return result;
        }
        public List<Vector2Int> GetAttackPositions(Vector2Int position, Actors owner) {

            List<Vector2Int> result = new();

            for (int i = 0; i < BoardHelpers.Mask4.Length; i++)
                if(IsInBounds(position + BoardHelpers.Mask4[i]))
                    if (tiles[position.x, position.y].unitID != "")
                        if(GetUnitReference(position).unitReference.owner != owner)
                            result.Add(position + BoardHelpers.Mask4[i]);
            return result;
        }
        public List<Vector2Int> GetSummonPositions(Actors actor) {
            List<Vector2Int> unitPositions = GetUnitPositions(actor);
            List<Vector2Int> result = new();   

            for (int i = 0; i < unitPositions.Count; i++)
                for (int j = 0; j < BoardHelpers.Mask8.Length; j++) {
                    Vector2Int pos = unitPositions[i] + BoardHelpers.Mask8[j];
                    if (IsInBounds(pos))
                        if (tiles[pos.x, pos.y].isFree)
                            result.Add(pos);
                }
            return result;
        }
        public List<Vector2Int> GetUnitPositions(Actors owner) {
            List<Vector2Int> unitPositions = new List<Vector2Int>();
            foreach (var item in GetActorReference(owner).GetLivingUnits())
                unitPositions.Add(item.unitPosition);
            return unitPositions;
        }
        #endregion

        #region UID
        public int currentUnitIndex; 
        public string GetUID() {
            currentUnitIndex += 1;
            return UnitIDManager.Instance.GetUID(currentUnitIndex - 1);
        }
        #endregion

        #region Helpers
        public UnitReference GetUnitReference(string ID) {
            return GetAllUnits().Find(unitRef => unitRef.unitID == ID);
        }
        public UnitReference GetUnitReference(Vector2Int pos) {
            return GetAllUnits().Find(unitRef => unitRef.unitPosition == pos);
        }
        public UnitReference GetUnitReference(int x, int y) {
            return GetAllUnits().Find(unitRef => unitRef.unitPosition == new Vector2Int(x,y));
        }
        public UnitReference GetUnitReference(Unit unit) {
            return GetAllUnits().Find(unitref => unitref.unitReference == unit);
        }
        public bool IsInBounds(Vector2Int pos)
        {
            return pos.x < tiles.GetLength(0) && pos.x >= 0 &&
                   pos.y < tiles.GetLength(1) && pos.y >= 0;
        }
        
        public List<UnitReference> GetAllUnits() {
            List<UnitReference> references = new();
            references.AddRange(Actor2_Deck.GetLivingUnits());
            references.AddRange(Actor1_Deck.GetLivingUnits());

            return references;
        }



        #endregion

        #region Hashing stuff
        public string GetJSON() {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(Actor1_Deck.GetHash());
            stringBuilder.Append(Actor2_Deck.GetHash());

            //stringBuilder.Append(JsonUtility.ToJson(this));
            for (int x = 0;x < tiles.GetLength(0);x++) {
                for (int y = 0;y < tiles.GetLength(1);y++) {
                    if (tiles[x,y].unitID == "") {
                        stringBuilder.Append("null |");
                    } else {
                        stringBuilder.Append(tiles[x, y].unitID);
                        stringBuilder.Append(" |");
                    }
                }
            }

            return stringBuilder.ToString();
        }

        public string GetHash() {
            return GetJSON();
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

        public Board GetCopy()
        {
            return new Board(this);
        }
        #endregion
    }
}

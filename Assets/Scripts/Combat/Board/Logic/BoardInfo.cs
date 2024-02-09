using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;

namespace Combat
{
    [System.Serializable]
    public struct BoardInfo
    {
        public BoardInfo(Board board, string BoardHash, Actors activeActor, List<ICommand> moves)
        {
            this.board = board;
            this.boardHash = BoardHash;
            this.actionsTaken = moves;
            this.activeActor = activeActor;
            this.resultingBoards = new();
        }


        //What actions were taken by whom
        public Actors activeActor;
        public List<ICommand> actionsTaken;

        //results from actions
        public Board board;
        public string boardHash;
        public List<string> resultingBoards;
    }
    [System.Serializable]
    public struct BoardEvaluation
    {
        public BoardEvaluation(int Eval, BoardInfo BoardInfo)
        {
            this.eval = Eval;
            this.boardInfo = BoardInfo;
        }
        public int eval;
        public BoardInfo boardInfo;

        public static BoardEvaluation Minimum => new(int.MinValue, new());
        public static BoardEvaluation Maximum => new(int.MaxValue, new());
    }
}

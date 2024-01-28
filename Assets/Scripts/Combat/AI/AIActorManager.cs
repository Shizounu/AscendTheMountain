using Cards;
using Combat;
using Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIActorManager : MonoBehaviour, IActorManager
{
    public DeckDefinition deck;

    public bool isEnabled => throw new System.NotImplementedException();

    public DeckInformation deckInformation => GameManager.Instance.currentBoard.getActorReference(Actors.Actor2);

    private void Start()
    {
        //Register for enabling
        deckInformation.actorManager = this;

        boardPool = new();

        GameManager.Instance.currentBoard.SetCommand(Command_InitSide.GetAvailable().Init(Actors.Actor2, deck));
        GameManager.Instance.currentBoard.DoQueuedCommands();
        GameManager.Instance.InitRootBoard();
    }

    public void Disable()
    {

    }
    public void Enable()
    {
        if(curBoard.curCommandCount > 0) {
            curBoard.SetCommand(Command_EnableSide.GetAvailable().Init(Actors.Actor1));
            return;
        }
        StartCoroutine(EnableActions());
        //EnableActions();
    }

    IEnumerator EnableActions() {
        //There to fix an issue with this activating before mana was applied and me having no clue how to fix it 
        yield return new WaitForEndOfFrame();

        //Check for state transitions
        foreach (var transition in CurState.transitions)
        {
            if (transition.Evaluate(GameManager.Instance.currentBoard))
            {
                CurState = transition.transitionTo;
                break;
            }
        }

        //PopulatePermutations(GameManager.Instance.currentBoard, Actors.Actor2);

        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();


        BoardEvaluation eval = EvaluateBoard(new BoardInfo("",new()), Actors.Actor2);

        sw.Stop();

        Debug.Log($"{sw.ElapsedMilliseconds}ms");
        Debug.Log(eval.eval);
    }

    #region Board Pool
    public BoardPool boardPool;
    public class BoardPool {
        public BoardPool(int count = 8) {
            AddToPool(count);
            capacity = count;
        }

        int capacity;
        Queue<Board> pool = new(); 

        public void AddToPool(int count) {
            for (int i = 0; i < count; i++)
                pool.Enqueue(new Board());
        }

        public Board GetFromPool(Board boardToCopy) {
            if(pool.Count == 0) {
                AddToPool(capacity);
                capacity *= 2;
            }
            Board board = pool.Dequeue();

            for (int x = 0; x < board.tiles.GetLength(0); x++)
                for (int y = 0; y < board.tiles.GetLength(1); y++)
                    board.tiles[x, y] = new Tile(boardToCopy.tiles[x, y]);
            board.Actor1_Deck = boardToCopy.Actor1_Deck.Clone();
            board.Actor2_Deck = boardToCopy.Actor2_Deck.Clone();
            board.onCommand = null;

            return board;
        }
        public void ReturnToPool(Board b) {
            pool.Enqueue(b);
        }
    }


    #endregion

    #region Board Generation
    Dictionary<string, BoardInfo> CachedBoards = new();
    Board curBoard = new();
    [Serializable]
    public struct BoardInfo {
        public BoardInfo(string preceedingBoard,List<ICommand> moves) {
            this.preceedingBoard = preceedingBoard;
            this.moves = moves;
            this.resultingBoards = new();
        }

        //Info
        public string preceedingBoard;

        public List<ICommand> moves;

        public List<string> resultingBoards;
    }

    private Command_EnableSide GetEnableSide(Actors currentActor) {
        return Command_EnableSide.GetAvailable().Init(currentActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1);
    }

    
    public Dictionary<string, BoardInfo> GetPossibleBoards(Board board, Actors currentActor, List<ICommand> actionsTaken = null, ICommand lastCommand = null, int curDepth = 30) {
        Dictionary<string, BoardInfo> resultingBoards = new();


        string baseBoardHash = board.GetHash();
        //Should only ever be relevant for the first ever board. Populates it into the dict
        if (!CachedBoards.ContainsKey(baseBoardHash)) {
            BoardInfo boardInfo = new BoardInfo("", (actionsTaken == null ? new() : actionsTaken));
            CachedBoards.Add(baseBoardHash, boardInfo);
            resultingBoards.Add(baseBoardHash, boardInfo);
        }

        List<ICommand> possibleMoves = GetPossibleActions(board, currentActor);
        List<ICommand> curActionsTaken = new();
        if (actionsTaken != null)
            curActionsTaken.AddRange(actionsTaken);
        if (lastCommand != null)
            curActionsTaken.Add(lastCommand);

        foreach (ICommand possibleMove in possibleMoves) {
            curBoard = boardPool.GetFromPool(board);
            curBoard.SetCommand(possibleMove);
            curBoard.SetCommand(GetEnableSide(currentActor));
            curBoard.DoQueuedCommands();
            string curBoardHash = curBoard.GetHash();

            if (CachedBoards.ContainsKey(curBoardHash))
                continue;


            BoardInfo boardInfo = new BoardInfo(baseBoardHash, curActionsTaken);
            CachedBoards[baseBoardHash].resultingBoards.Add(curBoardHash);
            CachedBoards.Add(curBoardHash, boardInfo);
            resultingBoards.Add(curBoardHash, boardInfo);

            if (curDepth <= 0)
                continue;

            resultingBoards.Merge(GetPossibleBoards(curBoard, currentActor, curActionsTaken, possibleMove, curDepth - 1));

        }
        boardPool.ReturnToPool(board);

        return resultingBoards;
    }

    public void PopulatePermutations(Board board, Actors currentActor, List<ICommand> actionsTaken = null, ICommand lastCommand = null, int curDepth = 30) {
        string baseBoardHash = board.GetHash();
        //Should only ever be relevant for the first ever board. Populates it into the dict
        if (!CachedBoards.ContainsKey(baseBoardHash)) {
            BoardInfo boardInfo = new BoardInfo("",(actionsTaken == null ? new() : actionsTaken));
            CachedBoards.Add(baseBoardHash, boardInfo);
        }

        List<ICommand> possibleMoves = GetPossibleActions(board, currentActor);
        List<ICommand> curActionsTaken = new();
        if(actionsTaken != null)
            curActionsTaken.AddRange(actionsTaken);
        if(lastCommand != null)
            curActionsTaken.Add(lastCommand);

        foreach (ICommand possibleMove in possibleMoves) {
            curBoard = boardPool.GetFromPool(board);
            curBoard.SetCommand(possibleMove);
            curBoard.SetCommand(GetEnableSide(currentActor));
            curBoard.DoQueuedCommands();
            string curBoardHash = curBoard.GetHash();

            if (CachedBoards.ContainsKey(curBoardHash))
                continue;

            
            BoardInfo boardInfo = new BoardInfo(baseBoardHash, curActionsTaken);
            CachedBoards[baseBoardHash].resultingBoards.Add(curBoardHash);
            CachedBoards.Add(curBoardHash, boardInfo);

            if (curDepth <= 0) 
                continue;

            PopulatePermutations(curBoard, currentActor, curActionsTaken, possibleMove, curDepth - 1);
        }
        boardPool.ReturnToPool(board);
    }

    public List<ICommand> GetPossibleActions(Board board, Actors activeActor)
    {
        List<ICommand> possibleActions = new List<ICommand>();

        //Summoning
        for (int i = 0; i < board.getActorReference(activeActor).Hand.Length; i++) {
            if (board.getActorReference(activeActor).Hand[i] != null) {
                if (board.getActorReference(activeActor).Hand[i].Cost <= board.getActorReference(activeActor).CurManagems) {
                    if (board.getActorReference(activeActor).Hand[i].GetType() == typeof(UnitDefinition)) {
                        List<Vector2Int> summonPositions = board.getSummonPositions(activeActor);
                        foreach (Vector2Int pos in summonPositions) {                           
                            possibleActions.Add(
                                Command_SummonUnit.GetAvailable().Init(
                                    (UnitDefinition)board.getActorReference(activeActor).Hand[i], pos, activeActor,
                                    false, false, true, true, i));
                        }
                    } else {
                        Debug.LogError("Unrecognized card type in AI hand");
                    }


                }
            }
        }


        List<Vector2Int> unitPositions = board.GetUnitPositions(activeActor);
        for (int i = 0; i < unitPositions.Count; i++) {
            //Move actions
            if (board.GetUnitFromPos(unitPositions[i]).canMove) {
                List<Vector2Int> movePositions = board.getMovePositions(unitPositions[i], activeActor, board.GetUnitFromPos(unitPositions[i]).moveDistance);
                foreach (Vector2Int movePos in movePositions) { 
                    possibleActions.Add(Command_MoveUnit.GetAvailable().Init(unitPositions[i], movePos));
                }
            }

            //Attack actions
            if (board.GetUnitFromPos(unitPositions[i]).canAttack) {
                List<Vector2Int> attackPositions = board.getAttackPositions(unitPositions[i]);
                foreach (Vector2Int attackPos in attackPositions)
                    if (board.GetUnitFromPos(attackPos) != null && board.GetUnitFromPos(attackPos).owner != activeActor) {
                        possibleActions.Add(Command_AttackUnit.GetAvailable().Init(board.GetUnitFromPos(unitPositions[i]), board.GetUnitFromPos(attackPos)));
                    }
            }
        }

        //Ending Turn
        //possibleActions.Add(new Command_EnableSide(activeActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1));

        return possibleActions;
    }
   
    
    public Board GetBoardFromInfo(BoardInfo boardInfo) {
        Stack<BoardInfo> infos = new();

        BoardInfo curInfo = boardInfo;
        do
        {
            infos.Push(curInfo);
            curInfo = CachedBoards[curInfo.preceedingBoard];
        } while (curInfo.preceedingBoard != "");

        Board board = GameManager.Instance.GetRootBoardCopy();
        while (infos.Count > 0) {
            curInfo = infos.Pop();
            foreach (var info in curInfo.moves) {
                board.SetCommand(info);
            }
            board.DoQueuedCommands();
        }
        return board;
    }
    #endregion

    #region Board Evaluation
    public Combat.AI.StateMachine.State CurState;

    public struct BoardEvaluation {
        public BoardEvaluation(int Eval, BoardInfo BoardInfo) {
            this.eval = Eval;
            this.boardInfo = BoardInfo;
        }
        public int eval;
        public BoardInfo boardInfo;
    }

    private Actors invertActor(Actors actor) {
        if (actor == Actors.Actor1)
            return Actors.Actor2;
        return Actors.Actor1;
    }

    public BoardEvaluation EvaluateBoard(BoardInfo curBoard, Actors maximizingActor, int alpha = int.MinValue, int beta = int.MaxValue,int depth = 3) {
        if(depth == 0 || curBoard.resultingBoards.Count == 0)
            return new(EvaluatePosition(curBoard), curBoard);

        if (maximizingActor == Actors.Actor2) {
            //Maximizes
            BoardEvaluation value = new BoardEvaluation(int.MinValue, new());
            Dictionary<string, BoardInfo> possibleMoves = GetPossibleBoards(GetBoardFromInfo(curBoard), maximizingActor);
            foreach (var item in possibleMoves) {
                BoardEvaluation cur = EvaluateBoard(item.Value, invertActor(maximizingActor), alpha, beta, depth - 1);
                if(cur.eval > value.eval)
                    value = cur;

                if (value.eval > beta)
                    break;
                alpha = Math.Max(value.eval, alpha);
            }
            return value;

        } else {
            //Minimizes
            BoardEvaluation value = new BoardEvaluation(int.MaxValue, new());
            Dictionary<string, BoardInfo> possibleMoves = GetPossibleBoards(GetBoardFromInfo(curBoard), maximizingActor);

            foreach (var item in possibleMoves) {
                BoardEvaluation cur = EvaluateBoard(item.Value, invertActor(maximizingActor), alpha, beta, depth - 1);
                if (cur.eval < value.eval)
                    value = cur;

                if (value.eval < alpha)
                    break;
                beta = Math.Min(value.eval, beta);
            }
            return value;
        }
    }

    public int EvaluatePosition(BoardInfo board)
    {
        int eval = 0;
        for (int i = 0; i < CurState.boardEvaluationConditions.Count; i++) {
            eval += CurState.boardEvaluationConditions[i].Evaluate(GetBoardFromInfo(board));
        }

        return eval;
    }
    #endregion
}
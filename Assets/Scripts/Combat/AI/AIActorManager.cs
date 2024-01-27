using Cards;
using Combat;
using Commands;
using System;
using System.Collections;
using System.Collections.Generic;
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

        PopulatePermutations(GameManager.Instance.currentBoard, Actors.Actor2);
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
        public BoardInfo(Board board, List<ICommand> moves) {
            this.moves = moves;
            this.board = board;
            this.resultingBoards = new();
        }

        //Info
        public Board board;
        public List<ICommand> moves;

        public List<string> resultingBoards;
    }

    //TODO: Update with optimizations made
    public Dictionary<string, BoardInfo> GetPossibleBoards(Board board, Actors currentActor, List<ICommand> actionsTaken = null, ICommand lastCommand = null, int curDepth = 30) {
        Dictionary<string, BoardInfo> resultingBoards = new();


        string baseBoardHash = board.GetHash();
        //Should only ever be relevant for the first ever board. Populates it into the dict
        if (!CachedBoards.ContainsKey(baseBoardHash)) {
            BoardInfo boardInfo = new BoardInfo(board, (actionsTaken == null ? new() : actionsTaken));
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
            curBoard.DoQueuedCommands();
            string curBoardHash = curBoard.GetHash();

            if (CachedBoards.ContainsKey(curBoardHash))
                continue;


            BoardInfo boardInfo = new BoardInfo(curBoard, curActionsTaken);
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
            BoardInfo boardInfo = new BoardInfo(board, (actionsTaken == null ? new() : actionsTaken));
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
            curBoard.DoQueuedCommands();
            string curBoardHash = curBoard.GetHash();

            if (CachedBoards.ContainsKey(curBoardHash))
                continue;

            
            BoardInfo boardInfo = new BoardInfo(curBoard, curActionsTaken);
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
    #endregion

    #region Board Evaluation
    public Combat.AI.StateMachine.State CurState;



    public int EvaluateBoard(BoardInfo board)
    {
        int eval = 0;
        for (int i = 0; i < CurState.boardEvaluationConditions.Count; i++) {
            eval += CurState.boardEvaluationConditions[i].Evaluate(board.board);
        }

        return eval;
    }
    #endregion
}
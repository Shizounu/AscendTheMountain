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
        //StartCoroutine(EnableActions());
        EnableActions();
    }

    void EnableActions() {
        //There to fix an issue with this activating before mana was applied and me having no clue how to fix it 
        //yield return new WaitForEndOfFrame();

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
    /* OBSOLETE
    /// <summary>
    /// WARNING!!!! : This works under the assumption that the hashes are actually properly unique. Not guranteed and could be a potential future error point
    /// </summary>
    
    public Dictionary<string, BoardInfo> GetPossibleBoards(Board baseBoard, Actors currentActor) {
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        timer.Start();

        Dictionary<string, BoardInfo> resultingBoards = new();
        List<(List<ICommand>, Board)> possibleTurns = GetPossibleTurns((baseBoard, new List<ICommand>()), currentActor, 30);
        foreach ((List<ICommand>, Board) turn in possibleTurns) {
            string boardHash = turn.Item2.GetHash();


            if (!resultingBoards.ContainsKey(boardHash)) {
                BoardInfo boardInfo = new BoardInfo(turn.Item2, turn.Item1, this);
                Debug.Log($"Added Unique Board, actions to get there: {turn.Item1.Count}");
                resultingBoards.Add(boardHash, boardInfo);
            } else {
                Debug.Log("Culled Dup");
            }
        }

        timer.Stop();
        Debug.LogWarning($"Time Elampsed {timer.ElapsedMilliseconds} ms");

        return resultingBoards;
    }
    
    public List<(List<ICommand>, Board)> GetPossibleTurns((Board, List<ICommand>) baseBoard, Actors currentActor, int curDepth = 30) {
        List<(List<ICommand>, Board)> results = new();


        List<ICommand> possibleMoves = GetPossibleActions(baseBoard.Item1, currentActor);
        foreach (ICommand possibleMove in possibleMoves) {
            Board curBoard = new Board(baseBoard.Item1);
            curBoard.SetCommand(possibleMove);
            curBoard.DoQueuedCommands();

            List<ICommand> commands = new List<ICommand>();
            commands.AddRange(baseBoard.Item2);
            commands.Add(possibleMove);

            if (possibleMove.GetType() == typeof(Command_EnableSide)) { //Enabling Other Side
                results.Add((commands, curBoard));
            } else if (curDepth == 0) {
                commands.Add(new Command_EnableSide(currentActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1));
                results.Add((commands, curBoard));
            } else {
                results.AddRange(GetPossibleTurns((curBoard, commands), currentActor, curDepth - 1));
            }
        }
        return results;
    }*/

    //TODO: Update with optimizations made
    /*public Dictionary<string, BoardInfo> GetPopulatePermutations(Board board, Actors currentActor, List<ICommand> actionsTaken = null, ICommand lastAction = null, int curDepth = 30) {
        Dictionary<string, BoardInfo> result = new();

        string baseBoardHash = board.GetHash();
        //Should only ever be relevant for the first ever board. Populates it into the dict
        if (!CachedBoards.ContainsKey(baseBoardHash)) {
            BoardInfo boardInfo = new BoardInfo(board, (actionsTaken == null ? new() : actionsTaken));
            CachedBoards.Add(baseBoardHash, boardInfo);
            result.Add(baseBoardHash, boardInfo);
        }

        List<ICommand> possibleMoves = GetPossibleActions(board, currentActor); 
        List<ICommand> curActionsTaken = new();
        if(actionsTaken != null)
            curActionsTaken.AddRange(CachedBoards[baseBoardHash].moves);
        if(lastAction != null)
            curActionsTaken.Add(lastAction);

        foreach (ICommand possibleMove in possibleMoves) {
            Board curBoard = new Board(board);
            curBoard.SetCommand(possibleMove);
            curBoard.DoQueuedCommands();

            string curBoardHash = curBoard.GetHash();
            if(!CachedBoards.ContainsKey(curBoardHash)) {
                BoardInfo boardInfo = new BoardInfo(curBoard, curActionsTaken);

                CachedBoards[baseBoardHash].resultingBoards.Add(curBoardHash);

                CachedBoards.Add(curBoardHash, boardInfo);
                result.Add(curBoardHash, boardInfo);

                Debug.Log($"Added Unique Board, actions to get there: {curActionsTaken.Count}");
                if (curDepth > 0){
                    result.Merge(GetPopulatePermutations(curBoard, currentActor, curActionsTaken, possibleMove, curDepth - 1));
                }
            }
        }
        return result;
    }*/
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

            Debug.Log($"Added Unique Board, actions to get there: {curActionsTaken.Count + 1}");
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
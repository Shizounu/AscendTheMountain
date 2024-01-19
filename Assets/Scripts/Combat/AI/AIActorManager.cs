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

        GameManager.Instance.currentBoard.SetCommand(new Command_InitSide(Actors.Actor2, deck));
        GameManager.Instance.currentBoard.DoQueuedCommands();
    }

    public void Disable()
    {

    }
    public void Enable()
    {
        //Delay cause instant start doesnt pass the right info for no apparent reason
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart(float time = 0.01f) {
        yield return new WaitForSecondsRealtime(time);
        EnableActions();
    }

    void EnableActions()
    {
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

    #region Command Pool
    public ObjectPool<Command_MoveUnit> CP_Move;
    public ObjectPool<Command_SummonUnit> CP_Summon;
    public ObjectPool<Command_AttackUnit> CP_Attack;
    public ObjectPool<Command_SetCanMove> CP_SetCanMove;
    public ObjectPool<Command_SetCanAttack> CP_SetCanAttack;
    public ObjectPool<Command_RemoveHandCard> CP_RemoveHandCard;
    public ObjectPool<Command_SubCurrentMana> CP_SubCurrentMana;

    public class ObjectPool<T> where T : ICommand
    {
        public ObjectPool(Action<ObjectPool<T>> addToPoolFunc, int defaultCount = 256)
        {
            pool = new();
            this.addToPoolFunc = addToPoolFunc;
            curCapacity = defaultCount;

            AddItemsToPool(curCapacity);
        }

        private Action<ObjectPool<T>> addToPoolFunc;
        private int curCapacity;
        public Queue<T> pool;



        public void AddItemsToPool(int c)
        {
            for (int i = 0; i < c; i++)
            {
                addToPoolFunc(this);
            }
        }

        public T GetFromPool()
        {
            if (pool.Count == 0)
            {
                Debug.LogWarning($"Pool of Type {pool.GetType()} is Empty. Doubling to {curCapacity * 2}");
                AddItemsToPool(curCapacity);
                curCapacity *= 2;
            }
            return pool.Dequeue();
        }

        public void ReturnToPool(T obj)
        {
            pool.Enqueue(obj);
        }
    }

    private void Awake()
    {
        CP_Move = new(x => x.pool.Enqueue(new Command_MoveUnit(new Vector2Int(), new Vector2Int())));
        CP_Summon = new(x => x.pool.Enqueue(new Command_SummonUnit(null, Vector2Int.zero, Actors.Actor1, false, false, true, true, 0)));
        CP_Attack = new(x => x.pool.Enqueue(new Command_AttackUnit(null, null)));

        CP_SetCanMove = new(x => x.pool.Enqueue(new Command_SetCanMove(null, false)));
        CP_SetCanAttack = new( x => x.pool.Enqueue(new Command_SetCanAttack(null, false)));
        CP_RemoveHandCard = new(x => x.pool.Enqueue(new Command_RemoveHandCard(0, Actors.Actor1)));
        CP_SubCurrentMana = new(x => x.pool.Enqueue(new Command_SubCurrentMana(Actors.Actor1, 0)));

        boardPool = new();
    }



    public void HandleCommandDisposal(ICommand command) {
        if(command.GetType() == typeof(Command_MoveUnit)) {
            CP_Move.ReturnToPool((Command_MoveUnit)command); return;
        }
        if(command.GetType() == typeof(Command_SummonUnit)) {
            CP_Summon.ReturnToPool((Command_SummonUnit)command); return;
        }
        if(command.GetType() == typeof(Command_AttackUnit)) {
            CP_Attack.ReturnToPool((Command_AttackUnit)command); return;
        }
        if(command.GetType() == typeof(Command_SetCanMove)) {
            CP_SetCanMove.ReturnToPool((Command_SetCanMove)command); return;
        }
        if (command.GetType() == typeof(Command_SetCanAttack)) {
            CP_SetCanAttack.ReturnToPool((Command_SetCanAttack)command); return;
        }
        if (command.GetType() == typeof(Command_RemoveHandCard)) {
            CP_RemoveHandCard.ReturnToPool((Command_RemoveHandCard)command); return;
        }
        if (command.GetType() == typeof(Command_SubCurrentMana)) {
            CP_SubCurrentMana.ReturnToPool((Command_SubCurrentMana)command); return;
        }


        Debug.LogWarning($"Deleted Other Command : {command.GetType()}");
        command = null;
    }

    #endregion

    #region Board Pool
    public BoardPool boardPool;
    public class BoardPool {
        public BoardPool(int count = 256) {
            AddToPool(count);
        }

        int curCount;
        Queue<Board> pool = new(); 

        public void AddToPool(int count) {
            for (int i = 0; i < count; i++)
                pool.Enqueue(new Board());
        }

        public Board GetFromPool(Board boardToCopy) {
            if(pool.Count == 0) {
                AddToPool(curCount);
                curCount *= 2;
            }
            Board board = pool.Dequeue();

            for (int x = 0; x < board.tiles.GetLength(0); x++)
                for (int y = 0; y < board.tiles.GetLength(1); y++)
                    board.tiles[x, y] = new Tile(boardToCopy.tiles[x, y]);
            board.Actor1_Deck = boardToCopy.Actor1_Deck.Clone();
            board.Actor2_Deck = boardToCopy.Actor2_Deck.Clone();

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

    /// <summary>
    /// Macro cause I got bored of writing this so often
    /// </summary>
    /// <param name="currentActor"></param>
    /// <returns></returns>
    private Command_EnableSide switchSideCommand(Actors currentActor) {
        return new Command_EnableSide(currentActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1);
    }
    //TODO: Update with optimizations made
    public Dictionary<string, BoardInfo> GetPopulatePermutations(Board board, Actors currentActor, List<ICommand> actionsTaken = null, int curDepth = 30) {
        Dictionary<string, BoardInfo> result = new();

        string baseBoardHash = board.GetHash();
        //Should only ever be relevant for the first ever board. Populates it into the dict
        if (!CachedBoards.ContainsKey(baseBoardHash)) {
            BoardInfo boardInfo = new BoardInfo(board, (actionsTaken == null ? new() : actionsTaken));
            CachedBoards.Add(baseBoardHash, boardInfo);
            result.Add(baseBoardHash, boardInfo);
        }

        List<ICommand> possibleMoves = GetPossibleActions(board, currentActor);
        foreach (ICommand possibleMove in possibleMoves) {
            Board curBoard = new Board(board);
            curBoard.SetCommand(possibleMove);
            curBoard.DoQueuedCommands();

            string curBoardHash = curBoard.GetHash();
            if(!CachedBoards.ContainsKey(curBoardHash)) {
                List<ICommand> curActionsTaken = new();
                curActionsTaken.AddRange(CachedBoards[baseBoardHash].moves);
                curActionsTaken.Add(possibleMove);
                BoardInfo boardInfo = new BoardInfo(curBoard, curActionsTaken);

                CachedBoards[baseBoardHash].resultingBoards.Add(curBoardHash);

                CachedBoards.Add(curBoardHash, boardInfo);
                result.Add(curBoardHash, boardInfo);

                Debug.Log($"Added Unique Board, actions to get there: {curActionsTaken.Count}");
                if (curDepth > 0){
                    result.Merge(GetPopulatePermutations(curBoard, currentActor, curActionsTaken, curDepth - 1));
                }
            }
        }
        return result;
    }
    public void PopulatePermutations(Board board, Actors currentActor, List<ICommand> actionsTaken = null, int curDepth = 30) {
        string baseBoardHash = board.GetHash();
        //Should only ever be relevant for the first ever board. Populates it into the dict
        if (!CachedBoards.ContainsKey(baseBoardHash)) {
            BoardInfo boardInfo = new BoardInfo(board, (actionsTaken == null ? new() : actionsTaken));
            CachedBoards.Add(baseBoardHash, boardInfo);
        }

        List<ICommand> possibleMoves = GetPossibleActions(board, currentActor);
        foreach (ICommand possibleMove in possibleMoves)
        {
            curBoard = boardPool.GetFromPool(board);
            curBoard.onCommand = HandleCommandDisposal;
            curBoard.SetCommand(possibleMove);
            curBoard.DoQueuedCommands();

            string curBoardHash = curBoard.GetHash();
            if (!CachedBoards.ContainsKey(curBoardHash)) {
                List<ICommand> curActionsTaken = new();
                curActionsTaken.AddRange(CachedBoards[baseBoardHash].moves);
                curActionsTaken.Add(possibleMove);
                BoardInfo boardInfo = new BoardInfo(curBoard, curActionsTaken);

                CachedBoards[baseBoardHash].resultingBoards.Add(curBoardHash);

                CachedBoards.Add(curBoardHash, boardInfo);

                Debug.Log($"Added Unique Board, actions to get there: {curActionsTaken.Count}");
                if (curDepth > 0) {
                    PopulatePermutations(curBoard, currentActor, curActionsTaken, curDepth - 1);
                }
            } else {
                //Debug.LogWarning("Found duplicate board, discarding");
            }
            boardPool.ReturnToPool(board);
        }
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
                            Command_SummonUnit com = CP_Summon.GetFromPool();

                            com.InitForPooling(
                                (UnitDefinition)board.getActorReference(activeActor).Hand[i], pos, activeActor,
                                false, false, true, true, i,
                                CP_SetCanMove.GetFromPool(), CP_SetCanAttack.GetFromPool(), CP_SubCurrentMana.GetFromPool(), CP_RemoveHandCard.GetFromPool()
                                );
                            
                            possibleActions.Add(com);
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
                List<Vector2Int> movePositions = board.getMovePositions(unitPositions[i], board.GetUnitFromPos(unitPositions[i]).moveDistance);
                foreach (Vector2Int movePos in movePositions) { 
                    Command_MoveUnit moveUnit = CP_Move.GetFromPool();

                    moveUnit.InitForPooling(unitPositions[i], movePos, CP_SetCanMove.GetFromPool());

                    possibleActions.Add(moveUnit);
                }
            }

            //Attack actions
            if (board.GetUnitFromPos(unitPositions[i]).canAttack) {
                List<Vector2Int> attackPositions = board.getAttackPositions(unitPositions[i]);
                foreach (Vector2Int attackPos in attackPositions)
                    if (board.GetUnitFromPos(attackPos) != null && board.GetUnitFromPos(attackPos).owner != activeActor) {
                        Command_AttackUnit attackUnit = CP_Attack.GetFromPool();

                        attackUnit.InitForPooling(board.GetUnitFromPos(unitPositions[i]), board.GetUnitFromPos(attackPos), CP_SetCanAttack.GetFromPool());

                        possibleActions.Add(attackUnit);
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
        return 0; //TODO write eval method
    }

    #endregion
}
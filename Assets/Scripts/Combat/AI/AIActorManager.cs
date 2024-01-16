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
        Debug.Log("Starting AI");
        //Delay cause instant start doesnt pass the right info for no reason
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
    private void Awake()
    {
        MoveCommandPool = new(x => x.ReturnToPool(new Command_MoveUnit(new Vector2Int(), new Vector2Int())));
        SummonCommandPool = new(x => x.ReturnToPool(new Command_SummonUnit(null, Vector2Int.zero, Actors.Actor1, false, false, true, true, 0)));
        AttackCommandPool = new(x => x.ReturnToPool(new Command_AttackUnit(null, null)));
    }

    public class ObjectPool<T> where T : ICommand {
        public ObjectPool(Action<ObjectPool<T>> addToPoolFunc, int defaultCount = 128) {
            pool = new();
            for (int i = 0; i < defaultCount; i++) {
                addToPoolFunc(this);
            }
        }

        private Queue<T> pool;

        public T GetFromPool() { 
            if(pool.Count == 0)
                Debug.LogError("Empty Move pool");
            return pool.Dequeue();
        }

        public void ReturnToPool(T obj) {
            pool.Enqueue(obj);
        } 
    }

    public ObjectPool<Command_MoveUnit> MoveCommandPool;
    public ObjectPool<Command_SummonUnit> SummonCommandPool;
    public ObjectPool<Command_AttackUnit> AttackCommandPool;
    public void HandleCommandDisposal(ICommand command) {
        if(command.GetType() == typeof(Command_MoveUnit)) {
            MoveCommandPool.ReturnToPool((Command_MoveUnit)command); return;
        }
        if(command.GetType() == typeof(Command_SummonUnit)) {
            SummonCommandPool.ReturnToPool((Command_SummonUnit)command); return;
        }
        if(command.GetType() == typeof(Command_AttackUnit)) {
            AttackCommandPool.ReturnToPool((Command_AttackUnit)command); return;
        }


        command = null;
    }

    #endregion

    #region Board Generation
    Dictionary<string, BoardInfo> CachedBoards = new();
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
        if (!CachedBoards.ContainsKey(baseBoardHash))
        {
            BoardInfo boardInfo = new BoardInfo(board, (actionsTaken == null ? new() : actionsTaken));
            CachedBoards.Add(baseBoardHash, boardInfo);
        }

        List<ICommand> possibleMoves = GetPossibleActions(board, currentActor);
        foreach (ICommand possibleMove in possibleMoves)
        {
            Board curBoard = new Board(board);
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
            }
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
                            Command_SummonUnit com = SummonCommandPool.GetFromPool();
                            com.position = pos;
                            com.unitDef = (UnitDefinition)board.getActorReference(activeActor).Hand[i];
                            com.owner = activeActor;
                            com.handIndex = i;
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
                    Command_MoveUnit moveUnit = MoveCommandPool.GetFromPool();
                    moveUnit.startPos = unitPositions[i];
                    moveUnit.path = new() { movePos };

                    possibleActions.Add(moveUnit);
                }
            }

            //Attack actions
            if (board.GetUnitFromPos(unitPositions[i]).canAttack) {
                List<Vector2Int> attackPositions = board.getAttackPositions(unitPositions[i]);
                foreach (Vector2Int attackPos in attackPositions)
                    if (board.GetUnitFromPos(attackPos) != null && board.GetUnitFromPos(attackPos).owner != activeActor) {
                        Command_AttackUnit attackUnit = AttackCommandPool.GetFromPool();
                        attackUnit.attacker = board.GetUnitFromPos(unitPositions[i]);
                        attackUnit.defender = board.GetUnitFromPos(attackPos);

                        possibleActions.Add(attackUnit);
                    }
            }
        }

        //Ending Turn
        //possibleActions.Add(new Command_EnableSide(activeActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1));

        return possibleActions;
    }
    

    [ContextMenu("Test Stuff")]
    public void Test() {
        CachedBoards.Clear();
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();    
        stopwatch.Start();
        PopulatePermutations(GameManager.Instance.currentBoard, Actors.Actor2);
        stopwatch.Stop();
        Debug.LogWarning($"Time Elapsed {stopwatch.ElapsedMilliseconds} ms. Added");

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
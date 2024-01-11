using Cards;
using Combat;
using Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class AIActorManager : MonoBehaviour, IActorManager
{
    public Combat.AI.StateMachine.State CurState;
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

        //Check for state transitions
        foreach (var transition in CurState.transitions) {
            if (transition.Evaluate(GameManager.Instance.currentBoard)) {
                CurState = transition.transitionTo;
                break;
            }
        }

        

    }

    #region Board Evaluation
    [Serializable]
    public class BoardInfo {
        public BoardInfo(List<ICommand> moves, Board board, AIActorManager manager) {
            this.moves  = moves;
            this.board = board;
            this.manager = manager;
        }

        //References
        public AIActorManager manager;
        
        //Info
        public List<ICommand> moves;
        public Board board;

        //Evaluation Logic
        private bool dirtyEvaluation = true; //Create logic for setting dirty flag
        private int cachedEvaluation; 

        public int GetEvaluation() {
            if(dirtyEvaluation) {
                manager.EvaluateBoard(board);
                dirtyEvaluation = false;
            }
            return cachedEvaluation;
        }

        //Optimization
        public List<string> resultingBoards = new();

    }

    /// <summary>
    /// WARNING!!!! : This works under the assumption that the hashes are actually properly unique. Not guranteed and could be a potential future error point
    /// </summary>
    public Dictionary<string, BoardInfo> GetPossibleBoards(Board baseBoard, Actors currentActor) {
        Dictionary<string, BoardInfo> resultingBoards = new();
        List<(List<ICommand>, Board)> possibleTurns = GetPossibleTurns((baseBoard, new List<ICommand>()), currentActor, 30);
        foreach ((List<ICommand>, Board) turn in possibleTurns) {
            string boardHash = turn.Item2.GetHash();


            if (!resultingBoards.ContainsKey(boardHash)) {
                BoardInfo boardInfo = new BoardInfo(turn.Item1, turn.Item2, this);
                Debug.Log($"Added Unique Board, actions to get there: {turn.Item1.Count}");
                resultingBoards.Add(boardHash, boardInfo);
            } else {
                Debug.Log("Culled Dup");
            }
        }

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
            } else if(curDepth == 0) {
                commands.Add(new Command_EnableSide(currentActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1));
                results.Add((commands, curBoard));
            }  else {
                results.AddRange(GetPossibleTurns((curBoard, commands), currentActor, curDepth - 1));
            }
        }
        return results;
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
                                new Command_SummonUnit(
                                    (UnitDefinition)board.getActorReference(activeActor).Hand[i],
                                    pos,
                                    activeActor,
                                    false, false, //Cant immediately attack or move
                                    true, true, i //Remove from hand & pay cost
                               )
                            );
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
                foreach (Vector2Int movePos in movePositions)
                    possibleActions.Add(new Command_MoveUnit(unitPositions[i], movePos));
            }

            //Attack actions
            if (board.GetUnitFromPos(unitPositions[i]).canAttack) {
                List<Vector2Int> attackPositions = board.getAttackPositions(unitPositions[i]);
                foreach (Vector2Int attackPos in attackPositions)
                    if (board.GetUnitFromPos(attackPos) != null && board.GetUnitFromPos(attackPos).owner != activeActor)
                        possibleActions.Add(new Command_AttackUnit(board.GetUnitFromPos(unitPositions[i]), board.GetUnitFromPos(attackPos)));
            }
        }

        //Ending Turn
        possibleActions.Add(new Command_EnableSide(activeActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1));

        return possibleActions;
    }
    
    public int EvaluateBoard(Board board)
    {
        return 0; //TODO write eval method
    }

    [ContextMenu("Test Stuff")]
    public void Test() {
        Dictionary<String, BoardInfo> boards = GetPossibleBoards(GameManager.Instance.currentBoard, Actors.Actor2);

    }
    #endregion
}
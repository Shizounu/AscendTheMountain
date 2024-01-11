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
    public struct BoardInfo {
        public BoardInfo(List<ICommand> moves, int boardEvaluation, List<Board> resultingBoards, Board board) {
            this.moves  = moves;
            this.boardEvaluation = boardEvaluation;
            this.resultingBoards = resultingBoards;
            this.board = board;
        }

        /// <summary>
        /// The moves the AI took to get here
        /// </summary>
        public List<ICommand> moves;
        public int boardEvaluation;
        public List<Board> resultingBoards;

        public Board board;
    }

    /// <summary>
    /// working variable for GetPossibleBoards function. 
    /// WARNING!!!! : This works under the assumption that the hashes are actually properly unique. Not guranteed and could be a potential future error point
    /// </summary>
    public Dictionary<string, BoardInfo> GetPossibleBoards(Board baseBoard, Actors currentActor) {
        Dictionary<string, BoardInfo> resultingBoards = new();
        List<List<ICommand>> possibleTurns = GetPossibleTurns((baseBoard, new List<ICommand>()), currentActor, 30);
        foreach (List<ICommand> turn in possibleTurns) {
            Board currentBoard = new Board(baseBoard);
            for (int i = 0; i < turn.Count; i++) {
                currentBoard.SetCommand(turn[i]);
            }
            currentBoard.DoQueuedCommands();
            string boardHash = currentBoard.GetHash();


            if (!resultingBoards.ContainsKey(boardHash)) {
                BoardInfo boardInfo = new BoardInfo(
                    turn,
                    EvaluateBoard(currentBoard),
                    new List<Board>(), //change to use hash strings
                    currentBoard
                    );

                Debug.Log($"Added Unique Board, actions to get there: {turn.Count}");
                resultingBoards.Add(currentBoard.GetHash(), boardInfo);
            } else {
                Debug.Log("Culled Dup");
            }
        }

        return resultingBoards;
    }
    //TODO: Also return already generated boards
    public List<List<ICommand>> GetPossibleTurns((Board, List<ICommand>) baseBoard, Actors currentActor, int curDepth = 30) {
        List<List<ICommand>> results = new List<List<ICommand>>();


        List<ICommand> possibleMoves = GetPossibleActions(baseBoard.Item1, currentActor);
        foreach (ICommand possibleMove in possibleMoves) {
            Board curBoard = new Board(baseBoard.Item1); 
            curBoard.SetCommand(possibleMove);
            curBoard.DoQueuedCommands();

            List<ICommand> commands = new List<ICommand>();
            commands.AddRange(baseBoard.Item2);
            commands.Add(possibleMove);

            if (possibleMove.GetType() == typeof(Command_EnableSide)) { //Enabling Other Side
                results.Add(commands);
            } else if(curDepth == 0) {
                commands.Add(new Command_EnableSide(currentActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1));
                results.Add(commands);
            }  else {
                results.AddRange(GetPossibleTurns((curBoard, commands), currentActor, curDepth - 1));
            }
        }
        return results;
    }

    public List<ICommand> GetPossibleActions(Board board, Actors activeActor)
    {
        List<ICommand> possibleActions = new List<ICommand>();

        //TODO: Invert check, to first check for if its a unit and then check generate & iterate through positions
        //Summoning
        List<Vector2Int> summonPositions = board.getSummonPositions(activeActor);
        for (int i = 0; i < board.getActorReference(activeActor).Hand.Length; i++) {
            if(board.getActorReference(activeActor).Hand[i] != null) {
                if (board.getActorReference(activeActor).Hand[i].Cost <= board.getActorReference(activeActor).CurManagems) {
                    foreach (Vector2Int pos in summonPositions) { 
                        if (board.getActorReference(activeActor).Hand[i].GetType() == typeof(UnitDefinition))
                        {
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
                        else //TODO add other card types
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
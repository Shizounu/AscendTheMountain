using Cards;
using Combat;
using Commands;
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

        GenerateMoves();

    }

    public List<ICommand> GenerateMoves() {
        List<ICommand> possibleInitialMoves = GetPossibleActions(GameManager.Instance.currentBoard, Actors.Actor2);

        Dictionary<Board, List<ICommand>> resultingBoards = new();

        foreach (var item in possibleInitialMoves) {
            Board b = new Board(GameManager.Instance.currentBoard);
            b.SetCommand(item);
            b.DoQueuedCommands();

            resultingBoards.Add(b, new List<ICommand> { item });
        }


        throw new System.NotImplementedException();
    }

    public List<ICommand> GetPossibleActions(Board board, Actors activeActor) {
        List<ICommand> possibleActions = new List<ICommand>();


        //Summoning
        List<Vector2Int> summonPositions = board.getSummonPositions(activeActor);
        for (int i = 0; i < board.getActorReference(activeActor).Hand.Length; i++) {
            if (board.getActorReference(activeActor).Hand[i]?.Cost <= board.getActorReference(activeActor).CurManagems) {
                
                foreach (Vector2Int pos in summonPositions)
                    if (board.getActorReference(activeActor).Hand[i].GetType() == typeof(UnitDefinition))
                        possibleActions.Add(
                            new Command_SummonUnit(
                                (UnitDefinition)board.getActorReference(activeActor).Hand[i], 
                                pos, 
                                activeActor,
                                false, false, true, true, i
                           )
                        );
                    else //TODO add other card types
                        Debug.LogError("Unrecognized card type in AI hand");

            }
        }

        //Moving & Attacking
        List<Vector2Int> unitPositions = board.GetUnitPositions(activeActor);
        for (int i = 0; i < unitPositions.Count; i++) {
            if (board.GetUnitFromPos(unitPositions[i]).canMove) {
                List<Vector2Int> movePositions = board.getMovePositions(unitPositions[i], board.GetUnitFromPos(unitPositions[i]).moveDistance);
                foreach (Vector2Int movePos in movePositions)
                    possibleActions.Add(new Command_MoveUnit(unitPositions[i], movePos));
            }

            if (board.GetUnitFromPos(unitPositions[i]).canAttack)
            {
                List<Vector2Int> attackPositions = board.getAttackPositions(unitPositions[i]);
                foreach (Vector2Int attackPos in attackPositions)
                    if (board.GetUnitFromPos(attackPos) != null)
                        possibleActions.Add(new Command_AttackUnit(board.GetUnitFromPos(unitPositions[i]), board.GetUnitFromPos(attackPos)));
            }
        }

        //Ending Turn
        possibleActions.Add(new Command_EnableSide(activeActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1));

        return possibleActions;
    }


    
}



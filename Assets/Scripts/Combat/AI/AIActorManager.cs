using Cards;
using Combat;
using Combat.Cards;
using Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Combat
{
    public class AIActorManager : MonoBehaviour, IActorManager
    {
        public DeckDefinition deck;

        public bool isEnabled => throw new System.NotImplementedException();

        public DeckInformation deckInformation => GameManager.Instance.currentBoard.GetActorReference(Actors.Actor2);

        public Board MainBoard => GameManager.Instance.currentBoard;

        #region Boilerplate
        private void Start()
        {
        

        }
        public void Init() {
            GameManager.Instance.currentBoard.SetCommand(Command_InitSide.GetAvailable().Init(Actors.Actor2, deck));
            GameManager.Instance.currentBoard.DoQueuedCommands();
        }
        public void Disable()
        {

        }
        public void Enable()
        {
            StartCoroutine(EnableActions());
            //EnableActions();
        }

        IEnumerator EnableActions() {
            //There to fix an issue with this activating before mana was applied and me having no clue how to fix it 
            yield return new WaitForEndOfFrame();

            //Check for state transitions
            foreach (var transition in CurState.transitions)
            {
                if (transition.Evaluate(new BoardInfo(MainBoard, MainBoard.GetHash(), Actors.Actor2, new())))
                {
                    CurState = transition.transitionTo;
                    break;
                }
            }


            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();


            //PopulatePermutations(GameManager.Instance.currentBoard, Actors.Actor2);
            BoardEvaluation eval = EvaluateBoard(new BoardInfo(MainBoard, MainBoard.GetHash(), Actors.Actor2, new()), Actors.Actor2);

            sw.Stop();

            Debug.Log($"{sw.ElapsedMilliseconds}ms");
            Debug.Log(eval.eval);
            string res = ""; 
            foreach (var item in eval.boardInfo.actionsTaken){
                res += $"{item} \n";
            }
            Debug.Log(res);

            MainBoard.SetCommand(eval.boardInfo.actionsTaken);
            MainBoard.SetCommand(GetEnableSide(Actors.Actor2));
            MainBoard.DoQueuedCommands();
        }
        #endregion

        #region Board Generation
        Dictionary<string, BoardInfo> CachedBoards = new();

        private Command_EndTurn GetEnableSide(Actors currentActor) {
            return Command_EndTurn.GetAvailable().Init(currentActor == Actors.Actor1 ? Actors.Actor2 : Actors.Actor1);
        }

    
        public Dictionary<string, BoardInfo> GetPossibleBoards(Board board, Actors currentActor, List<ICommand> actionsTaken = null, int curDepth = 30) {
            Dictionary<string, BoardInfo> resultingBoards = new();


            string baseBoardHash = board.GetHash();
            BoardInfo baseBoardInfo = new BoardInfo(board, baseBoardHash, currentActor, (actionsTaken == null ? new() : actionsTaken));
            resultingBoards.Add(baseBoardHash, baseBoardInfo);

            List<ICommand> possibleMoves = GetPossibleActions(board, currentActor);

            foreach (ICommand possibleMove in possibleMoves) {
                //Generate Board resulting from this move
                Board curBoard = board.GetCopy();
                curBoard.SetCommand(possibleMove);
                curBoard.SetCommand(GetEnableSide(currentActor));
                curBoard.DoQueuedCommands();
                string curBoardHash = curBoard.GetHash();

                if (resultingBoards.ContainsKey(curBoardHash))
                    continue;

                List<ICommand> curActionsTaken = new();
                if(actionsTaken != null)
                    curActionsTaken.AddRange(actionsTaken);
                curActionsTaken.Add(possibleMove);
                BoardInfo boardInfo = new BoardInfo(curBoard, curBoardHash, currentActor, curActionsTaken);
                
                resultingBoards.Add(curBoardHash, boardInfo);

                if (curDepth <= 0)
                    continue;

                resultingBoards.Merge(GetPossibleBoards(curBoard, currentActor, curActionsTaken, curDepth - 1));

            }

            return resultingBoards;
        }

        public void PopulatePermutations(Board board, Actors currentActor, List<ICommand> actionsTaken = null, ICommand lastCommand = null, int curDepth = 30) {
            string baseBoardHash = board.GetHash();
            //Should only ever be relevant for the first ever board. Populates it into the dict
            if (!CachedBoards.ContainsKey(baseBoardHash)) {
                BoardInfo boardInfo = new BoardInfo(board, baseBoardHash,currentActor, (actionsTaken == null ? new() : actionsTaken));
                CachedBoards.Add(baseBoardHash, boardInfo);
            }

            List<ICommand> possibleMoves = GetPossibleActions(board, currentActor);
            List<ICommand> curActionsTaken = new();
            if(actionsTaken != null)
                curActionsTaken.AddRange(actionsTaken);
            if(lastCommand != null)
                curActionsTaken.Add(lastCommand);

            foreach (ICommand possibleMove in possibleMoves) {
                //Generate Board resulting from this move
                Board curBoard = board.GetCopy();
                curBoard.SetCommand(possibleMove);
                curBoard.SetCommand(GetEnableSide(currentActor));
                curBoard.DoQueuedCommands();
                string curBoardHash = curBoard.GetHash();

                if (CachedBoards.ContainsKey(curBoardHash))
                    continue;

                BoardInfo boardInfo = new BoardInfo(curBoard, curBoardHash, currentActor, curActionsTaken);
                CachedBoards[baseBoardHash].resultingBoards.Add(curBoardHash);
                CachedBoards.Add(curBoardHash, boardInfo);

                //Debug.Log($"Added board with move count {curActionsTaken.Count + 1}");
                if (curDepth <= 0) 
                    continue;

                PopulatePermutations(curBoard, currentActor, curActionsTaken, possibleMove, curDepth - 1);
            }
        }

        public List<ICommand> GetPossibleActions(Board board, Actors activeActor)
        {
            List<ICommand> possibleActions = new();

            //Summoning
            for (int i = 0; i < board.GetActorReference(activeActor).Hand.Length; i++) {
                if (board.GetActorReference(activeActor).Hand[i] == null)
                    continue;
                
                if (board.GetActorReference(activeActor).Hand[i].cardCost > board.GetActorReference(activeActor).CurManagems)
                    continue;    
                
                if (board.GetActorReference(activeActor).Hand[i].GetType() == typeof(CardInstance_Unit)) {
                    List<Vector2Int> summonPositions = board.GetSummonPositions(activeActor);
                    foreach (Vector2Int pos in summonPositions) {                           
                        possibleActions.Add(
                            Command_SummonUnit.GetAvailable().Init(
                                (CardInstance_Unit)board.GetActorReference(activeActor).Hand[i], pos, activeActor,
                                false, false, true, true, i));
                    }
                } else {
                    Debug.LogError("Unrecognized card type in AI hand");
                }
            }


            List<Vector2Int> unitPositions = board.GetUnitPositions(activeActor);
            for (int i = 0; i < unitPositions.Count; i++) {
                //Move actions
                if (board.GetUnitReference(unitPositions[i]).unitReference.canMove) {
                    List<Vector2Int> movePositions = board.GetMovePositions(unitPositions[i], activeActor, board.GetUnitReference(unitPositions[i]).unitReference.moveDistance);
                    foreach (Vector2Int movePos in movePositions) { 
                        possibleActions.Add(Command_MoveUnit.GetAvailable().Init(unitPositions[i], movePos));
                    }
                }

                //Attack actions
                if (board.GetUnitReference(unitPositions[i]).unitReference.canAttack) {
                    List<Vector2Int> attackPositions = board.GetAttackPositions(unitPositions[i], activeActor);
                    foreach (Vector2Int attackPos in attackPositions)
                        if (board.GetUnitReference(attackPos) != null && board.GetUnitReference(attackPos).unitReference.owner != activeActor) {
                            possibleActions.Add(Command_AttackUnit.GetAvailable().Init(board.GetUnitReference(unitPositions[i]).unitReference.UnitID, board.GetUnitReference(attackPos).unitReference.UnitID));
                        }
                }
            }

            return possibleActions;
        }

        #endregion

        #region Board Evaluation
        public Combat.AI.StateMachine.State CurState;

        private Actors InvertActor(Actors actor) {
            if (actor == Actors.Actor1)
                return Actors.Actor2;
            return Actors.Actor1;
        }

        public BoardEvaluation EvaluateBoard(BoardInfo curBoard, Actors currentActor, int alpha = int.MinValue, int beta = int.MaxValue,int depth = 3) {
            if(depth == 0)
                return new(EvaluatePosition(curBoard), curBoard); //Return evaluation of the current board

            if (currentActor == Actors.Actor2) {
                //Maximizes
                BoardEvaluation currentBest = BoardEvaluation.Minimum; //sets evaluation to smalles value possible so everything is larger
            
                Dictionary<string, BoardInfo> possibleMoves = GetPossibleBoards(curBoard.board, currentActor);
                foreach (var item in possibleMoves) {
                    BoardEvaluation cur = EvaluateBoard(item.Value, InvertActor(currentActor), alpha, beta, depth - 1);
                    if(cur.eval > currentBest.eval) //checks if current is better than best
                        currentBest = cur; //current becomes new best if its best

                    if (currentBest.eval > beta) //Move is so good it wouldnt be let through
                        break;
                    alpha = Math.Max(currentBest.eval, alpha);
                }
                return currentBest;

            } else {
                //Minimizes
                BoardEvaluation currentBest = BoardEvaluation.Maximum;
                Dictionary<string, BoardInfo> possibleMoves = GetPossibleBoards(curBoard.board, currentActor);

                foreach (var item in possibleMoves) {
                    BoardEvaluation cur = EvaluateBoard(item.Value, InvertActor(currentActor), alpha, beta, depth - 1);
                    if (cur.eval < currentBest.eval)
                        currentBest = cur;

                    if (currentBest.eval < alpha) //Move is so bad it wouldnt be let through
                        break;
                    beta = Math.Min(currentBest.eval, beta);
                }
                return currentBest;
            }
        }

        public int EvaluatePosition(BoardInfo board) {
            int eval = 0;
            for (int i = 0; i < CurState.boardEvaluationConditions.Count; i++) {
                eval += CurState.boardEvaluationConditions[i].Evaluate(board) * CurState.boardEvaluationConditions[i].Weight;
            }

            return eval;
        }
        #endregion
    }
}
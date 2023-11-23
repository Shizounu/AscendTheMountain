using Cards;
using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;
using System.Threading;

public class PlayerActorManager : Shizounu.Library.SingletonBehaviour<PlayerActorManager>, IActorManager {

    public bool isEnabled { get; set; }

    public DeckDefinition PlayerDeck;
    
    public DeckInformation deckInformation => GameManager.Instance.currentBoard.Actor1_Deck;

    #region Input Functions
    [SerializeReference, Editor.SubclassPicker] public InputStates.InputState currentState;
    public void OnTileSelect(Vector2Int position)
    {
        if(!isEnabled) {
            //Do proper informing player of lock
            Debug.Log("Not able to control");
            return;
        }
        currentState.OnTileSelect(this, position);
    }
    public void OnHandSelect(int handIndex)
    {
        if (!isEnabled)
        {
            //Do proper informing player of lock
            Debug.Log("Not able to control");
            return;
        }
        currentState.OnHandSelect(this, handIndex);
    }
    public void OnCancel()
    {
        if (!isEnabled)
        {
            //Do proper informing player of lock
            Debug.Log("Not able to control");
            return;
        }
        currentState.OnCancel(this);
    }
    #endregion


    private void Start()
    {
        //Register for Enabling
        GameManager.Instance.currentBoard.Actor1_Deck.actorManager = this;


        //Init deck
        GameManager.Instance.currentBoard.SetCommand(new Command_SetDeck(PlayerDeck.Cards, Actors.Actor1));
        GameManager.Instance.currentBoard.SetCommand(new Command_DrawCard(Actors.Actor1, 3));


        //Initialize Input
        currentState = new InputStates.InputState_Default();
        Input.InputManager.Instance.InputActions.BattlefieldControls.RightClick.performed += ctx => OnCancel();


        GameManager.Instance.currentBoard.SetCommand(new Command_SwitchSide(Actors.Actor2));
        GameManager.Instance.currentBoard.DoQueuedCommands();
    }



    public void Enable()
    {
        //Init Input
        currentState = new InputStates.InputState_Default();
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) {
            currentState?.DrawGizmos(this);
        }
    }
}

namespace InputStates
{
    [System.Serializable]
    public abstract class InputState
    {
        protected Board currentBoard => GameManager.Instance.currentBoard;

        public abstract void OnTileSelect(PlayerActorManager sm, Vector2Int position);
        public abstract void OnHandSelect(PlayerActorManager sm, int handIndex);
        public abstract void OnCancel(PlayerActorManager sm);

        public abstract void DrawGizmos(PlayerActorManager sm);
    }

    [System.Serializable]
    public class InputState_Default : InputState
    {
        public override void DrawGizmos(PlayerActorManager sm)
        {
        }

        public override void OnCancel(PlayerActorManager sm)
        {
            sm.currentState = new InputState_Default();
        }

        public override void OnHandSelect(PlayerActorManager sm, int handIndex)
        {
            sm.currentState = new InputState_HandCardSelected(handIndex);
        }

        public override void OnTileSelect(PlayerActorManager sm, Vector2Int position)
        {
            //If selected tile has a Unit -> Transfer into that unit control
            if (currentBoard.tiles[position.x, position.y].unit != null) {
                sm.currentState = new InputState_UnitSelected(position);
            }
            //If selected tile has no Unit -> Remain here
        }
    }

    [System.Serializable]
    public class InputState_HandCardSelected : InputState {
        public InputState_HandCardSelected(int selectedCard) {
            curCard = selectedCard;
        }
        private int curCard;
        public override void OnCancel(PlayerActorManager sm)
        {
            //Clean hand stuff

            sm.currentState = new InputState_Default();

        }

        public override void OnHandSelect(PlayerActorManager sm, int handIndex)
        {
            curCard = handIndex;
        }

        public override void OnTileSelect(PlayerActorManager sm, Vector2Int position)
        {
            //needs more sophisticated check
            if (currentBoard.tiles[position.x, position.y].unit == null &&
                currentBoard.Actor1_Deck.CurManagems >= currentBoard.Actor1_Deck.Hand[curCard].Cost) {
                //dispatch summon command 
                currentBoard.SetCommand
                    (
                        new Commands.Command_SummonUnit(
                            (UnitDefinition) sm.deckInformation.Hand[curCard], 
                            position, 
                            Actors.Actor1
                        )
                    );
                //dispatch "remove from hand" command
                currentBoard.SetCommand
                    (
                        new Commands.Command_RemoveHandCard
                        (
                            curCard, 
                            Actors.Actor1
                        )
                    );

                currentBoard.SetCommand
                    (
                        new Commands.Command_SubCurrentMana
                        (
                            Actors.Actor1,
                            currentBoard.Actor1_Deck.Hand[curCard].Cost
                        )
                    );

                currentBoard.DoQueuedCommands();

                //return to a base state
                sm.currentState = new InputState_Default();
            } else {
                //TODO: Throw visual error
                
                Debug.Log("Cant summon there or the cost too high");
            }
        }

        public override void DrawGizmos(PlayerActorManager sm)
        {
        }
    }

    public class InputState_UnitSelected : InputState {
        public InputState_UnitSelected(Vector2Int unitPosition) {
            this.unitPosition = unitPosition;
        }
        Vector2Int unitPosition;
        Unit unit => currentBoard.tiles[unitPosition.x, unitPosition.y].unit;
        public override void OnCancel(PlayerActorManager sm)
        {
            sm.currentState = new InputState_Default();
        }

        public override void OnHandSelect(PlayerActorManager sm, int handIndex)
        {
            sm.currentState = new InputState_HandCardSelected(handIndex);
        }
        private bool isAvailablePos(Vector2Int position) {
            List<Vector2Int> positions = currentBoard.getMovePositions(unitPosition, unit.moveDistance);
            foreach (var pos in positions) 
                if(position == pos)
                    return true;
            return false;
        }
        public override void OnTileSelect(PlayerActorManager sm, Vector2Int position)
        {
            if (!(unit.canMove || unit.canAttack)) {
                sm.currentState = new InputState_Default();
                return;
            }


            if (unit.canMove && currentBoard.getMovePositions(unitPosition, unit.moveDistance).Contains(position))
            {
                currentBoard.SetCommand(new Commands.Command_MoveUnit(unitPosition, position));
                currentBoard.SetCommand(new Commands.Command_SetCanMove(unit, false));
                currentBoard.DoQueuedCommands();

                sm.currentState = new InputState_Default();
                return;
            }

            List<Vector2Int> attackPos = new List<Vector2Int>() {
                unitPosition + Vector2Int.left,
                unitPosition + Vector2Int.right,
                unitPosition + Vector2Int.up,
                unitPosition + Vector2Int.down
            };
            if(unit.canAttack && attackPos.Contains(position)) {
                if (currentBoard.tiles[position.x, position.y].unit != null && currentBoard.tiles[position.x, position.y].unit.owner != Actors.Actor1) {
                    currentBoard.SetCommand(new Command_DamageUnit(unit.attack, currentBoard.tiles[position.x, position.y].unit));

                    currentBoard.DoQueuedCommands();
                } 
            } else {
                sm.currentState = new InputState_Default();
                return;
            }


        }

        public override void DrawGizmos(PlayerActorManager sm)
        {
            if (unit.canMove)
            {
                List<Vector2Int> positions = currentBoard.getMovePositions(unitPosition, 2);
                Gizmos.color = Color.blue;
                Vector3 offset = new Vector3(BoardRenderer.Instance.transform.position.x + BoardRenderer.Instance.positionOffset.x, BoardRenderer.Instance.transform.position.y + BoardRenderer.Instance.positionOffset.y);
                foreach (Vector2Int pos in positions) {
                    Vector3 position = new Vector3(pos.x * BoardRenderer.Instance.tileScale.x, pos.y * BoardRenderer.Instance.tileScale.y);
                    Gizmos.DrawWireCube(position + offset, new Vector3(BoardRenderer.Instance.tileScale.x, BoardRenderer.Instance.tileScale.y, 0));
                }
            }
            if (unit.canAttack)
            {
                List<Vector2Int> attackPos = new List<Vector2Int>() {
                    unitPosition + Vector2Int.left,
                    unitPosition + Vector2Int.right,
                    unitPosition + Vector2Int.up,
                    unitPosition + Vector2Int.down
                };
                Vector3 offset = new Vector3(BoardRenderer.Instance.transform.position.x + BoardRenderer.Instance.positionOffset.x, BoardRenderer.Instance.transform.position.y + BoardRenderer.Instance.positionOffset.y);
                Gizmos.color = Color.red;
                foreach (var pos in attackPos)
                {
                    Vector3 position = new Vector3(pos.x * BoardRenderer.Instance.tileScale.x, pos.y * BoardRenderer.Instance.tileScale.y);
                    Gizmos.DrawWireCube(position + offset, new Vector3(BoardRenderer.Instance.tileScale.x, BoardRenderer.Instance.tileScale.y, 0));
                }
            }
            

        }
    }
}
using Cards;
using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;

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
        GameManager.Instance.currentBoard.SetCommand(new Command_SetDeck(PlayerDeck.Cards, Actors.Actor1));
        GameManager.Instance.currentBoard.SetCommand(new Command_DrawCard(Actors.Actor1, 3));
        GameManager.Instance.currentBoard.DoQueuedCommands();


        //Initialize Input
        currentState = new InputStates.InputState_Default();
        Input.InputManager.Instance.InputActions.BattlefieldControls.RightClick.performed += ctx => OnCancel();

        

        Enable();
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

}

namespace InputStates
{
    [System.Serializable]
    public abstract class InputState
    {
        public abstract void OnTileSelect(PlayerActorManager sm, Vector2Int position);
        public abstract void OnHandSelect(PlayerActorManager sm, int handIndex);
        public abstract void OnCancel(PlayerActorManager sm);
    }

    [System.Serializable]
    public class InputState_Default : InputState
    {
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
            Debug.Log($"The tile {position} was selected");
            //If selected tile has a Unit -> Transfer into that unit control
            Unit unit = GameManager.Instance.currentBoard.tiles[position.x, position.y].unit;
            if (unit != null) {
                sm.currentState = new InputState_UnitSelected(unit);
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
            if (GameManager.Instance.currentBoard.tiles[position.x, position.y].unit == null) {
                //dispatch summon command 
                GameManager.Instance.currentBoard.SetCommand
                    (
                        new Commands.Command_SummonUnit(
                            (UnitDefinition) sm.deckInformation.Hand[curCard], 
                            position, 
                            Actors.Actor1
                        )
                    );
                //dispatch "remove from hand" command
                GameManager.Instance.currentBoard.SetCommand
                    (
                        new Commands.Command_RemoveHandCard
                        (
                            curCard, 
                            Actors.Actor1
                        )
                    );

                GameManager.Instance.currentBoard.DoQueuedCommands();

                //return to a base state
                sm.currentState = new InputState_Default();
            } else {
                //TODO: Throw visual error
                
                Debug.Log("Cant summon there");
            }
        }
    }

    public class InputState_UnitSelected : InputState {
        public InputState_UnitSelected(Unit _unit) {
            unit = _unit;
        }
        Unit unit;
        public override void OnCancel(PlayerActorManager sm)
        {
            sm.currentState = new InputState_Default();
        }

        public override void OnHandSelect(PlayerActorManager sm, int handIndex)
        {
            throw new System.NotImplementedException();
        }

        public override void OnTileSelect(PlayerActorManager sm, Vector2Int position)
        {
            throw new System.NotImplementedException();
        }
    }
}
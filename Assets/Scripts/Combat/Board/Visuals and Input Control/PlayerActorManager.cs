using Cards;
using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;
using Combat.Cards;

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
        Debug.Log("canceled out of state");
    }
    #endregion

#if UNITY_EDITOR
    public UnitDefinition testingDefinition;
#endif

    private void Start()
    {
        //Initialize Input
        currentState = new InputStates.InputState_Default();
        Input.InputManager.Instance.InputActions.BattlefieldControls.RightClick.performed += ctx => OnCancel();


    }

    public void Init()
    {
        GameManager.Instance.currentBoard.SetCommand(Command_InitSide.GetAvailable().Init(Actors.Actor1, PlayerDeck));
        GameManager.Instance.currentBoard.SetCommand(Command_EndTurn.GetAvailable().Init(Actors.Actor1)); 

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
            if (currentBoard.tiles[position.x, position.y].unitID != "" && currentBoard.GetUnitReference(position).unitReference.owner == Actors.Actor1) {
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
            if (currentBoard.tiles[position.x, position.y].unitID == "" &&
                currentBoard.Actor1_Deck.CurManagems >= currentBoard.Actor1_Deck.Hand[curCard].cardCost &&
                currentBoard.GetSummonPositions(Actors.Actor1).Contains(position)) {
                //dispatch summon command 
                currentBoard.SetCommand
                    (
                        Command_SummonUnit.GetAvailable().Init(
                            (CardInstance_Unit) sm.deckInformation.Hand[curCard], 
                            position, 
                            Actors.Actor1,
                            false,
                            false,
                            true,
                            true,
                            curCard
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
            List<Vector2Int> summonPositions = currentBoard.GetSummonPositions(Actors.Actor1);
            Gizmos.color = Color.green;
            Vector3 offset = new Vector3(BoardRenderer.Instance.transform.position.x + BoardRenderer.Instance.positionOffset.x, BoardRenderer.Instance.transform.position.y + BoardRenderer.Instance.positionOffset.y);
            foreach (Vector2Int pos in summonPositions)
            {
                Vector3 position = new Vector3(pos.x * BoardRenderer.Instance.tileScale.x, pos.y * BoardRenderer.Instance.tileScale.y);
                Gizmos.DrawWireCube(position + offset, new Vector3(BoardRenderer.Instance.tileScale.x, BoardRenderer.Instance.tileScale.y, 0));
            }
        }
    }

    [System.Serializable]
    public class InputState_UnitSelected : InputState {
        public InputState_UnitSelected(Vector2Int unitPosition) {
            this.unitPosition = unitPosition;
        }
        Vector2Int unitPosition;
        Unit unit => GameManager.Instance.currentBoard.GetUnitReference(unitPosition).unitReference;
        public override void OnCancel(PlayerActorManager sm)
        {
            sm.currentState = new InputState_Default();
        }

        public override void OnHandSelect(PlayerActorManager sm, int handIndex)
        {
            sm.currentState = new InputState_HandCardSelected(handIndex);
        }
        private bool isAvailablePos(Vector2Int position) {
            List<Vector2Int> positions = currentBoard.GetMovePositions(unitPosition,Actors.Actor1 ,unit.moveDistance);
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


            if (unit.canMove && currentBoard.GetMovePositions(unitPosition, Actors.Actor1,unit.moveDistance).Contains(position))
            {

                currentBoard.SetCommand(Command_MoveUnit.GetAvailable().Init(unitPosition, position));

                sm.currentState = new InputState_Default();
                currentBoard.DoQueuedCommands();
                return;
            }

            ///TODO: potential rare bug where you can attack using an enemy unit? Havent been able to reproduce

            if( unit.canAttack && currentBoard.GetAttackPositions(unitPosition, Actors.Actor1).Contains(position))
            {
                if (currentBoard.tiles[position.x, position.y].unitID != "" &&
                    currentBoard.GetActorReference(Actors.Actor1).GetLivingUnits().Exists(unitRef => unitRef.unitID == currentBoard.tiles[position.x, position.y].unitID)) {
                    
                    currentBoard.SetCommand(Command_AttackUnit.GetAvailable().Init(unit.UnitID, currentBoard.GetUnitReference(position).unitID));

                    currentBoard.DoQueuedCommands();
                    Debug.Log("Attacked");
                } 
            } else {
                sm.currentState = new InputState_Default();
                Debug.Log("Not attacking");
                return;
            }


        }

        public override void DrawGizmos(PlayerActorManager sm)
        {
            if (unit.canMove)
            {
                List<Vector2Int> positions = currentBoard.GetMovePositions(unitPosition, Actors.Actor1,2);
                Gizmos.color = Color.blue;
                Vector3 offset = new Vector3(BoardRenderer.Instance.transform.position.x + BoardRenderer.Instance.positionOffset.x, BoardRenderer.Instance.transform.position.y + BoardRenderer.Instance.positionOffset.y);
                foreach (Vector2Int pos in positions) {
                    Vector3 position = new Vector3(pos.x * BoardRenderer.Instance.tileScale.x, pos.y * BoardRenderer.Instance.tileScale.y);
                    Gizmos.DrawWireCube(position + offset, new Vector3(BoardRenderer.Instance.tileScale.x, BoardRenderer.Instance.tileScale.y, 0));
                }
            }
            if (unit.canAttack)
            {
                
                Vector3 offset = new Vector3(BoardRenderer.Instance.transform.position.x + BoardRenderer.Instance.positionOffset.x, BoardRenderer.Instance.transform.position.y + BoardRenderer.Instance.positionOffset.y);
                Gizmos.color = Color.red;
                foreach (var pos in currentBoard.GetAttackPositions(unitPosition, Actors.Actor1))
                {
                    Vector3 position = new Vector3(pos.x * BoardRenderer.Instance.tileScale.x, pos.y * BoardRenderer.Instance.tileScale.y);
                    Gizmos.DrawWireCube(position + offset, new Vector3(BoardRenderer.Instance.tileScale.x, BoardRenderer.Instance.tileScale.y, 0));
                }
            }
            

        }
    }
}
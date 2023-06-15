using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Input Controller for managing the battlefield 

//Statemachine swapping between the different possible states

namespace Battlefield.Controller
{
    public class Controller : MonoBehaviour
    {
        [Header("Values")]
        public Vector2Int selectedTile = new Vector2Int(-1,-1);
        public CardDefinition selectedCard = null;

        [Header("References")]
        public Board board;
        public Deck deck;
        public Input.InputActions inputActions;

        [Header("Statemachine")]
        public ControllerState currentState;



        private void Awake() {
            //get input actions from game manager later, for now just done for quick
            inputActions = new();
            inputActions.BattlefieldControl.Enable();
            
            inputActions.BattlefieldControl.LeftClick.performed += ctx => onLeftClick();
            inputActions.BattlefieldControl.RightClick.performed += ctx => onRightClick();
        }

        private void Update() {
            foreach (StateTransition transition in currentState.transitions){
                if(transition.transitionCondition.condition(this)){
                    currentState = transition.transitionToState;
                    break;
                }
            }
        }

        private void onLeftClick(){
            currentState.onLeftClick(this);
        }
        private void onRightClick(){
            currentState.onRightClick(this);
        }
        public void selectHandCard(int index){
            selectedCard = deck.hand[index];
        }
    
        private void OnEnable() {
            //nputActions.BattlefieldControl.Enable();
        }
        private void OnDisable() {
            inputActions.BattlefieldControl.Disable();
        }
    }
    
}


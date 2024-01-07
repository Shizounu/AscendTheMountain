using Combat;
using Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActorManager : IActorManager
{
    public Combat.AI.StateMachine.State CurState;


    public bool isEnabled => throw new System.NotImplementedException();

    public DeckInformation deckInformation => GameManager.Instance.currentBoard.Actor2_Deck;

    public void Disable()
    {
        throw new System.NotImplementedException();
    }

    public void Enable()
    {
        //Check for state transitions
        foreach (var transition in CurState.transitions) {
            if (transition.Evaluate(GameManager.Instance.currentBoard)) {
                CurState = transition.transitionTo;
                break;
            }
        }

    }
}

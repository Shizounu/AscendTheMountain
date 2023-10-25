using Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorManager : MonoBehaviour {
    
    public Cards.CardDefinition[] Hand;
    public List<CardDefinition> Deck;

    public abstract void Enable();
    public abstract void Disable();

    
    
}

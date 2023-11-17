using Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public interface IActorManager {
        public bool isEnabled { get; }
        public abstract DeckInformation deckInformation { get;  }

        void Enable();
        void Disable();

    
    
    }

    [System.Serializable]

    public class DeckInformation
    {
        public List<CardDefinition> Deck = new();
        public CardDefinition[] Hand = new Cards.CardDefinition[6];
    }

}

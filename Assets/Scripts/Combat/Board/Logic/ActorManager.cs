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

    public class DeckInformation {
        public DeckInformation(DeckDefinition definition) {
            Deck = definition.Cards;
        }

        public List<CardDefinition> Deck = new();
        public CardDefinition[] Hand = new Cards.CardDefinition[6];

        public int getFreeHandIndex() {
            for (int i = 0; i < Hand.Length; i++) {
                if (Hand[i] == null)
                    return i;
            }
            return -1;
        }
    }

}

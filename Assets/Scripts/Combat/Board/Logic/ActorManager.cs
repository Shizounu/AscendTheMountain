using Cards;
using System;
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
        [Header("Mana")]
        [SerializeField] private int _MaxManagems;
        public int MaxManagems
        {
            get => _MaxManagems;
            set
            {
                _MaxManagems = value;
                _MaxManagems = Math.Clamp(_MaxManagems, 0, 10);
                CurManagems = CurManagems; //There to update mana to fit new max mana
            }

        }

        [SerializeField] private int _CurManagems;
        public int CurManagems
        {
            get => _CurManagems;
            set
            {
                _CurManagems = value;
                _CurManagems = Math.Clamp(_CurManagems, 0, MaxManagems);
            }

        }

        [Header("Cards")]
        public List<CardDefinition> Deck = new();
        public CardDefinition[] Hand = new Cards.CardDefinition[6];

        public int getFreeHandIndex() {
            for (int i = 0; i < Hand.Length; i++) {
                if (Hand[i] == null)
                    return i;
            }
            return -1;
        }

        public IActorManager actorManager;
        public void Enable() => actorManager?.Enable();
        public void Disable() => actorManager?.Disable();
    
        public DeckInformation Clone() {
            DeckInformation newDeck = new DeckInformation();
            newDeck._MaxManagems = _MaxManagems;
            newDeck._CurManagems = _CurManagems;
            
            newDeck.Deck = new List<CardDefinition>();
            foreach (CardDefinition card in Deck)
                newDeck.Deck.Add(card.Clone());

            newDeck.Hand = new CardDefinition[]
            {
                Hand[0]?.Clone(),
                Hand[1]?.Clone(),
                Hand[2]?.Clone(),
                Hand[3]?.Clone(),
                Hand[4]?.Clone(),
                Hand[5]?.Clone(),
            };

            return newDeck;
        }
    }

}

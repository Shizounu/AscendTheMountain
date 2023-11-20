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
        public DeckInformation(DeckDefinition definition) {
            Deck = definition.Cards;
        }


        private int _MaxManagems;
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

        private int _CurManagems;
        public int CurManagems
        {
            get => _CurManagems;
            set
            {
                _CurManagems = value;
                _CurManagems = Math.Clamp(_CurManagems, 0, MaxManagems);
            }

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Combat.Cards;

namespace Combat
{
    public interface IActorManager {
        public bool isEnabled { get; }
        public abstract DeckInformation deckInformation { get;  }

        void Init();
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
        public List<CardInstance> Deck = new();
        public CardInstance[] Hand = new CardInstance[6];

        public int getFreeHandIndex() {
            for (int i = 0; i < Hand.Length; i++) {
                if (Hand[i] == null)
                    return i;
            }
            return -1;
        }

        [Header("Units")]
        public Unit General; // TODO: Better way of referencing units than by a direct reference, ID stuff
        public List<Unit> currentUnits;
        
    }

}

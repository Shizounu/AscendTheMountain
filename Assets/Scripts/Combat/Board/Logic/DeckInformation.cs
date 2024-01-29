using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Combat.Cards;

namespace Combat
{
    [System.Serializable]

    public class DeckInformation : ICopyable<DeckInformation> {
        public DeckInformation() { }
        private DeckInformation(DeckInformation deckInformation) {
            this.MaxManagems = deckInformation.MaxManagems;
            this.CurManagems = deckInformation.CurManagems;

            this.Deck = new();
            foreach (var item in deckInformation.Deck)
                this.Deck.Add(GetCardCopy(item));
            
            this.Hand = new CardInstance[6];
            for (var i = 0; i < 6; i++) 
                this.Hand[i] = GetCardCopy(deckInformation.Hand[i]);
            

        }
        private CardInstance GetCardCopy(CardInstance instance) {
            if(instance?.GetType() == typeof(CardInstance_Unit)) {
                return ((CardInstance_Unit)instance).GetCopy();
            }


            return null; 
        }


        [Header("Mana")]
        [SerializeField] private int _MaxManagems;
        public int MaxManagems
        {
            get => _MaxManagems;
            set {
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

        //[Header("Units")]
        // TODO: Create way of indexing units

        public DeckInformation GetCopy() {
            return new DeckInformation(this);
        }
    }

}

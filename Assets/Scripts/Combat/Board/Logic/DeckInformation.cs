using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Combat.Cards;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Combat
{
    [System.Serializable]
    public class DeckInformation : ICopyable<DeckInformation> {
        public DeckInformation() {
            LivingUnitReferences = new();
            Deck = new();
            Hand = new CardInstance[6];
        }
        private DeckInformation(DeckInformation deckInformation) {
            this.MaxManagems = deckInformation.MaxManagems;
            this.CurManagems = deckInformation.CurManagems;

            this.Deck = new();
            foreach (var item in deckInformation.Deck)
                this.Deck.Add(GetCardCopy(item));
            
            this.Hand = new CardInstance[6];
            for (var i = 0; i < 6; i++) 
                this.Hand[i] = GetCardCopy(deckInformation.Hand[i]);

            this.GeneralReference = deckInformation.GeneralReference.GetCopy();
            this.LivingUnitReferences = new();
            foreach (var item in deckInformation.LivingUnitReferences)
                this.LivingUnitReferences.Add(item.GetCopy());
        }
        private CardInstance GetCardCopy(CardInstance instance) {
            if(instance == null) return null;
            if(instance.GetType() == typeof(CardInstance_Unit)) return ((CardInstance_Unit)instance).GetCopy();

            throw new System.NullReferenceException();
        }

        #region Mana
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
        #endregion

        #region Cards
        public List<CardInstance> Deck = new();
        public CardInstance[] Hand = new CardInstance[6];
        #endregion

        #region Unit Reference
        public UnitReference GeneralReference;
        private List<UnitReference> LivingUnitReferences = new();
        public List<UnitReference> GetLivingUnits()
        {
            List<UnitReference> refs = new List<UnitReference>();
            //Should fire only once when the first actor gets initiated
            if (GeneralReference != null) {
                refs.Add(GeneralReference);
            }

            refs.AddRange(LivingUnitReferences);
            return refs;
        }
        public void AddLivingUnitReference(UnitReference unitRef) {
            LivingUnitReferences.Add(unitRef);
        }
        public void AddLivingUnitReference(string unitID, Unit unitInstance, Vector2Int unitPosition) {
            LivingUnitReferences.Add(new UnitReference(unitID, unitInstance, unitPosition));
        }
        #endregion

        #region Helper Functions
        public int getFreeHandIndex() {
            for (int i = 0; i < Hand.Length; i++) {
                if (Hand[i] == null)
                    return i;
            }
            return -1;
        }
        public DeckInformation GetCopy() {
            return new DeckInformation(this);
        }
        public string GetHash() {
            return JsonUtility.ToJson(this);
        }
        #endregion
    }

    [System.Serializable]
    public class UnitReference : ICopyable<UnitReference> {
        public UnitReference(string unitID, Unit unitReference, Vector2Int unitPosition) {
            this.unitID = unitID;
            this.unitPosition = unitPosition;
            this.unitReference = unitReference;
        }
        private UnitReference(UnitReference unitRef) {
            this.unitID = unitRef.unitID;
            this.unitPosition = unitRef.unitPosition;

            this.unitReference = unitRef.unitReference.GetCopy();
        }


        public string unitID;
        public Unit unitReference;
        public Vector2Int unitPosition; 

        public void ChangePosition(Vector2Int newPosition) {
            unitPosition = newPosition;
        }

        public UnitReference GetCopy() {
            return new UnitReference(this);
        }
    }

}

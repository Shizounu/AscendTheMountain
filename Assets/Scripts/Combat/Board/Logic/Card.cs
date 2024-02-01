using Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.Cards
{

    [System.Serializable]
    public abstract class CardInstance {
        public CardInstance(CardDefinition definition) {
            this.cardName = definition.name;
            this.cardIcon = definition.Icon;
            this.cardAnimator = definition.animatorController;

            this.cardCost = definition.Cost;
        }
        public CardInstance(CardInstance instance) {
            this.cardName=instance.cardName;
            this.cardIcon=instance.cardIcon;
            this.cardAnimator = instance.cardAnimator;
            this.cardCost = instance.cardCost;
        }
        
        //Fluff
        public string cardName;
        public RuntimeAnimatorController cardAnimator;
        public Sprite cardIcon;

        //Relevant for Logic
        public int cardCost;
    }

    public class CardInstance_Unit : CardInstance, ICopyable<CardInstance_Unit> {
        public CardInstance_Unit(UnitDefinition definition) : base (definition) {
            this.UnitHealth = definition.Health;
            this.UnitAttack = definition.Attack;
            this.UnitMoveDistance = definition.MoveDistance;
        }
        public CardInstance_Unit(CardInstance_Unit instance) : base(instance) {
            this.UnitHealth = instance.UnitHealth;
            this.UnitAttack = instance.UnitAttack;
            this.UnitMoveDistance = instance.UnitMoveDistance;
        }
        

        public int UnitHealth;
        public int UnitAttack;
        public int UnitMoveDistance;

        public CardInstance_Unit GetCopy() {
            return new CardInstance_Unit(this);
        }
    }
}
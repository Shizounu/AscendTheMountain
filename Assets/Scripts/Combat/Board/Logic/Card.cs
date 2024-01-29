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
        
        //Fluff
        public string cardName;
        public RuntimeAnimatorController cardAnimator;
        public Sprite cardIcon;

        //Relevant for Logic
        public int cardCost;


    }

    public class CardInstance_Unit : CardInstance {
        public CardInstance_Unit(UnitDefinition definition) : base (definition) {
            this.UnitHealth = definition.Health;
            this.UnitAttack = definition.Attack;
            this.UnitMoveDistance = definition.MoveDistance;
        }

        public int UnitHealth;
        public int UnitAttack;
        public int UnitMoveDistance; 
    }
}
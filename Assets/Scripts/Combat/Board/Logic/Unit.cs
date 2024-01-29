using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cards;
using Commands;
using Combat.Cards;

namespace Combat
{
    [System.Serializable]
    public class Unit {
        public Unit(CardInstance_Unit definition, Actors owner) {
            maxHealth = definition.UnitHealth;
            curHealth = definition.UnitHealth;

            attack = definition.UnitAttack;

            moveDistance = definition.UnitMoveDistance;

            canMove = false;

            this.owner = owner;
        }
        public Unit(Unit unitToCopy){
            maxHealth = unitToCopy.maxHealth;
            curHealth = unitToCopy.curHealth;
            attack = unitToCopy.attack;
            moveDistance = unitToCopy.moveDistance;
            
            canMove = unitToCopy.canMove;
            canAttack = unitToCopy.canAttack;
            
            owner = unitToCopy.owner;

        }

        private int _curHealth;
        public int curHealth {
            get => _curHealth;
            set {
                _curHealth = value;
                if (_curHealth > maxHealth)
                    _curHealth = maxHealth;
            }
        }

        public Actors owner;

        public int maxHealth;

        public int attack;
        public bool canAttack;

        public int moveDistance;
        public bool canMove;

        ///TODO: Set up triggers and info for the viaul system to use. 
 
        public string GetJSON(bool prettyPrint = false) {
            return JsonUtility.ToJson(this, prettyPrint);
        }
    }
}

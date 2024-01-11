using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cards;
using Commands;

namespace Combat
{
    [System.Serializable]
    public class Unit {
        public Unit(UnitDefinition definition, Actors owner) {
            maxHealth = definition.Health;
            curHealth = definition.Health;

            attack = definition.Attack;

            moveDistance = definition.MoveDistance;

            effects = definition.effects;

            canMove = false;

            this.owner = owner;
        }
        public Unit(Unit unitToCopy){
            maxHealth = unitToCopy.maxHealth;
            curHealth = unitToCopy.curHealth;
            attack = unitToCopy.attack;
            moveDistance = unitToCopy.moveDistance;
            effects = unitToCopy.effects;
            
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

        public List<IEffect> effects;

        ///TODO: Set up triggers and info for the viaul system to use. 
    }
}

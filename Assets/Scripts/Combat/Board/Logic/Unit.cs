using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cards;
using Commands;

namespace Combat
{
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

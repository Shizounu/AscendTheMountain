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
            
        }

        private int _curHealth;
        public int curHealth {
            get => _curHealth;
            set {
                _curHealth = value;
                if(_curHealth <= 0) {

                }
                if (_curHealth > maxHealth)
                    _curHealth = maxHealth;
            }
        }
        public Actors owner;

        public int maxHealth;

        public int attack;

        public int moveDistance;

        public List<IEffect> effects;


    }
}

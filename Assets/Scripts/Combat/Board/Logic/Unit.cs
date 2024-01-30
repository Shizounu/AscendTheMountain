using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cards;
using Commands;
using Combat.Cards;
using UnityEditor;
using System;

namespace Combat
{
    [System.Serializable]
    public class Unit : ICopyable<Unit> {
        public Unit(CardInstance_Unit definition, Actors owner) {
            maxHealth = definition.UnitHealth;
            curHealth = definition.UnitHealth;

            attack = definition.UnitAttack;

            moveDistance = definition.UnitMoveDistance;

            canMove = false;

            this.owner = owner;

            UnitID = Guid.NewGuid().ToString();
        }
        public Unit(Unit unitToCopy){
            maxHealth = unitToCopy.maxHealth;
            curHealth = unitToCopy.curHealth;
            attack = unitToCopy.attack;
            moveDistance = unitToCopy.moveDistance;
            
            canMove = unitToCopy.canMove;
            canAttack = unitToCopy.canAttack;
            
            owner = unitToCopy.owner;

            UnitID = unitToCopy.UnitID;

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

        public readonly string UnitID;

        public Actors owner;

        public int maxHealth;

        public int attack;
        public bool canAttack;

        public int moveDistance;
        public bool canMove;

        
 
        public string GetJSON(bool prettyPrint = false) {
            return JsonUtility.ToJson(this, prettyPrint);
        }

        public Unit GetCopy() {
            return new Unit(this);
        }
    }
}

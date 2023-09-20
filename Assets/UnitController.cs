using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class UnitController : MonoBehaviour {
        
        [Header("Stats")]
        public int MaxHealth;
        [SerializeField] private int _Health;
        public int Health{
            get => _Health;
            set {
                _Health = value;
                if(_Health > MaxHealth)
                    _Health = MaxHealth;
                if(_Health <= 0)
                    Debug.Log("Death");
            }
        }
        public int Attack;
        public int MoveDistance;
    

    }
}

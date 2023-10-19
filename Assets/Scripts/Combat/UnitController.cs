using Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class UnitController : MonoBehaviour {
        private SideManager manager;

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

        public Vector2Int currentPosition;

        [Header("References")]
        public UnitVisualsManager VisualsManager;
        public void AddMaxHealth(int val)
        {
            MaxHealth += val;
            Health += val;
        }

        public void Initialize(UnitDefinition _definition, Vector2Int _startPos, SideManager _manager) {
            AddMaxHealth(_definition.Health);
            Attack = _definition.Attack;
            MoveDistance = _definition.MoveDistance;

            currentPosition = _startPos;
            manager = _manager;

            
            VisualsManager.Init(_definition.animatorController);
        }

        public void Move(Vector2Int movePos) {
            manager.GameManager.currentMap[currentPosition.x, currentPosition.y].asossciatedUnit = null;
            transform.position = manager.GameManager.currentMap[currentPosition.x, currentPosition.y].transform.position;
            manager.GameManager.currentMap[movePos.x, movePos.y].asossciatedUnit = this;
        }


    }
}

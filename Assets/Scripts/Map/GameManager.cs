using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Events;
using Shizounu.Library.ScriptableArchitecture;

namespace Map{
    public class GameManager : MonoBehaviour
    {
        [Header("Player")]
        public IntReference maxHealth;
        public IntReference curHealth;
        public UnityEvent OnDeath;

        [Space()]
        public IntReference money;
        [Space()]
        public Node currentNode;
        public UnityEvent OnFinalNode;

        [Header("References")]
        [SerializeField] private MapGenerator mapGenerator;

        [SerializeField] private bool hasRaisedFinalFlag = false;

        private void Start()
        {
            mapGenerator.GenerateMap();
        }

        private void Update()
        {
            if (!hasRaisedFinalFlag && IsNonContinuable())
            {
                OnFinalNode.Invoke();
                hasRaisedFinalFlag = true;
            }
        }

        private bool IsNonContinuable()
        {
            if (currentNode == null)
                return false;
            return currentNode.connectedNodes.Count == 0;
        }

    }
}
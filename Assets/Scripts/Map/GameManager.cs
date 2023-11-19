using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Events;
using Shizounu.Library.ScriptableArchitecture;
using UnityEngine.SceneManagement;
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

        [Space(), Header("Scene Managment")]
        public IntReference mapSceneIndex;
        [Space()]
        public ScriptableEvent OnDisableMap;
        public ChangeMapActivityListener disableListener;
        [Space()]
        public ScriptableEvent OnEnableMap;
        public ChangeMapActivityListener enableListener;
        
        [Header("References")]
        [SerializeField] private MapGenerator mapGenerator;

        [SerializeField] private bool hasRaisedFinalFlag = false;

        private void Start()
        {
            mapGenerator.GenerateMap();
            disableListener = new ChangeMapActivityListener(OnDisableMap, () => SetActiveMap(false));
            enableListener = new ChangeMapActivityListener(OnEnableMap, () => SetActiveMap(true));
        }

        private void Update()
        {
            if (!hasRaisedFinalFlag && IsNonContinuable())
            {
                OnFinalNode.Invoke();
                hasRaisedFinalFlag = true;
            }
        }

        private void SetActiveMap(bool value){
            Scene s = SceneManager.GetSceneByBuildIndex(mapSceneIndex);
            GameObject[] rootObjects = s.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; i++) 
                rootObjects[i].SetActive(value);
        }

        private bool IsNonContinuable()
        {
            if (currentNode == null)
                return false;
            return currentNode.connectedNodes.Count == 0;
        }

    }
}
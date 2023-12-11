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
        public List<GameObject> toDisable;
        public ScriptableEvent OnDisableMap;
        public ScriptableEvent OnEnableMap;
        
        [Header("References")]
        [SerializeField] private MapGenerator mapGenerator;

        [SerializeField] private bool hasRaisedFinalFlag = false;

        private void Start()
        {
            mapGenerator.GenerateMap();
            OnDisableMap += new SwitchCam(false, toDisable);
            OnEnableMap += new SwitchCam(true, toDisable);

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

    public class SwitchCam : IScriptableEventListener {
        public SwitchCam(bool state, List<GameObject> disable) {
            this.state = state;
            this.disable = disable;
        }
        bool state;
        List<GameObject> disable;

        public void EventResponse() {
            foreach (var item in disable) {
                item.SetActive(state);
            }
        }
    }
}
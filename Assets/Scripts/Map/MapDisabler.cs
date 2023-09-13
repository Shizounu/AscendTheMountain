using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shizounu.Library.ScriptableArchitecture;
namespace Map {
    public class MapDisabler : MonoBehaviour
    {
        public IntReference mapIndex;
        public ScriptableEvent onSceneLoad;

        
        public void SetActiveScene(int sceneIndex,bool val){
            GameObject[] rootObjects = SceneManager.GetSceneByBuildIndex(sceneIndex).GetRootGameObjects();
            foreach (var root in rootObjects)
                root.SetActive(val);
        }
    }

}

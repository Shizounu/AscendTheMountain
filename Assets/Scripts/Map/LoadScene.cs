using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using Shizounu.Library.ScriptableArchitecture;

namespace Map {
    public class LoadScene : MonoBehaviour
    {
        public IntReference sceneIndex;
        public ScriptableEvent eventReference;    
        public void DoSwitchScene(){
            StartCoroutine(ActivateOnFinishLoad(
                SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive),
                eventReference
            ));
        }
        private IEnumerator ActivateOnFinishLoad(AsyncOperation operation, ScriptableEvent scriptableEvent){
            yield return new WaitUntil(() => operation.isDone);
            Debug.Log("Scene Finished Loading");
            scriptableEvent.Invoke();
        }
    }
}

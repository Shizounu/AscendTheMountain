using Shizounu.Library.ScriptableArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnloadScene : MonoBehaviour
{
    public ScriptableInt sceneIndex;
    public ScriptableEvent enableMap; 

    public void DoUnloadScene()
    {
        SceneManager.UnloadSceneAsync(sceneIndex.runtimeValue);
        enableMap.Invoke();
    }
}

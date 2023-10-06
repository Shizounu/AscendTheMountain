using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Shizounu.Library.ScriptableArchitecture;

namespace Map {
    public class ChangeMapActivityListener : IScriptableEventListener
    {
        public ChangeMapActivityListener(ScriptableEvent e, System.Action action)
        {
            e.RegisterListener(this);
            disableMap = action;
        }

        System.Action disableMap;

        public void EventResponse()
        {
            disableMap.Invoke();
        }
    }

}

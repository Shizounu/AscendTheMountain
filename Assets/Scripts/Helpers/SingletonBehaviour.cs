using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shizounu.Library
{
    /// <summary>
    /// Singleton Monobehaviour Instance 
    /// Generic type T is to self replace the name of the class
    /// 
    /// If implementing an Awake function, use Base.Awake() at the top to still retain monobehaviour functionality
    /// 
    /// https://forum.unity.com/threads/singleton-monobehaviour-script.99971/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new System.Exception("An Instance of this singleton already exists.");
            }
            else
            {
                Instance = (T)this;
            }
        }
    }
}



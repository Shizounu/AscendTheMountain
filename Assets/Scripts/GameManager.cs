using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; protected set;}
    private void Awake() {
        if(Instance != null && Instance != this){
            Destroy(this);
            Debug.LogError("Additional game manager was instanitated. additional manager wad deleted");
            return;
        }
        Instance = this;



        Physics2D.queriesHitTriggers = true;
        InputActions = new();
    }

    public Input.InputActions InputActions;
}

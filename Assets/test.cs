using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using Map.Events;
public class test : MonoBehaviour
{
    private void Start() {
        MapEvent e = new MapEvent("TestEvent", 
            new List<MapEventSlide>(){
                new MapEventSlide("test test 123", 
                    new List<MapEventAction>(){
                        new MapEventAction ("testing proceeds", 
                        new List<ActionLogic>(){
                            new ActionLogic(Actions.RemoveHealth, 1)
                        })
                    }
                )
            }
        );

        Debug.Log(MapEvent.GetJson(e));
        Debug.Log(e.GetJson());
    }
}

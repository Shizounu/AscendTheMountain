using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map.Events {
    [System.Serializable]
    public class MapEvent {
        public MapEvent(string _EventName, List<MapEventSlide> _Slides){
            EventName = _EventName;
            Slides = _Slides;
        }
        public string EventName;

        public List<MapEventSlide> Slides;

        public static string GetJson(MapEvent mapEvent){
            return JsonUtility.ToJson(mapEvent, true);
        }
        public string GetJson(){
            return JsonUtility.ToJson(this, true);
        }
        public static MapEvent FromJson(string JsonObject){
            return JsonUtility.FromJson<MapEvent>(JsonObject);
        }
    }

    [System.Serializable]
    public class MapEventSlide {
        public MapEventSlide(string _Text, List<MapEventAction> _mapEventActions){
            Text = _Text;
            mapEventActions = _mapEventActions;
        }
        public string Text;
        public List<MapEventAction> mapEventActions;
    }

    [System.Serializable]
    public class MapEventAction {
        public MapEventAction(string _ActionText, List<ActionLogic> _ActionLogics){
            ActionText = _ActionText;
            ActionLogics = _ActionLogics;
        }
        public string ActionText;
        public List<ActionLogic> ActionLogics; 
    }
    [System.Serializable]
    public class ActionLogic {
        public ActionLogic(Actions _action, int _value){
            Action = _action;
            Value = _value;
        }
        public Actions Action;
        public int Value;
    } 
    public enum Actions {
        GoToSlide,
        //Health
        AddHealth,
        RemoveHealth,
        //Money
        AddMoney,
        RemoveMoney
    }

}

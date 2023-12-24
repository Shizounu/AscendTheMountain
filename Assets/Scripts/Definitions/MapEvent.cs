using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Map.Events {
    [System.Serializable]
    public class MapEvent {
        public MapEvent(string _EventName, List<MapEventSlide> _Slides, string _InitialSlide)
        {
            EventName = _EventName;
            Slides = _Slides;
            InitialSlide = _InitialSlide;
        }
        public string EventName;

        public List<MapEventSlide> Slides;

        /// <summary>
        /// UUID Casted as string corresponding to the slide ID 
        /// </summary>
        public string InitialSlide;
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
        public MapEventSlide(string _Text, List<MapEventAction> _mapEventActions, string ID){
            Text = _Text;
            mapEventActions = _mapEventActions;
            SlideID = ID;
        }
        public string SlideID;
        public string Text;
        public List<MapEventAction> mapEventActions;
    }

    [System.Serializable]
    public class MapEventAction {
        public MapEventAction(string _ActionText, List<MapEventActionLogic> _ActionLogics){
            ActionText = _ActionText;
            ActionLogics = _ActionLogics;
        }
        public string ActionText;
        public List<MapEventActionLogic> ActionLogics; 
    }
    [System.Serializable]
    public class MapEventActionLogic
    {
        public MapEventActionLogic(Actions _action, string _value){
            Action = _action;
            Value = _value;
        }
        public Actions Action;
        public string Value;
    }
    [System.Serializable]
    public enum Actions {
        GoToSlide,
        //Health
        AddHealth,
        RemoveHealth,
        //Money
        AddMoney,
        RemoveMoney,

        Exit
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battlefield.Controller
{

    public abstract class ControllerState : ScriptableObject 
    {
        public abstract void onLeftClick(Controller controller);
        public abstract void onRightClick(Controller controller);

        public List<StateTransition> transitions;
    }

    [System.Serializable]
    public class StateTransition{
        public ControllerState transitionToState;
        public TransitionCondition transitionCondition;
    }
    
}
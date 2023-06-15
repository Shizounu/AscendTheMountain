using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlefield.Controller
{
    public abstract class TransitionCondition : ScriptableObject
    {
        public abstract bool condition(Controller controller);
    }
    
}

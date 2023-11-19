using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Input {
    /// <summary>
    /// Singleton class holding a reference to the global class 'input actions'
    /// </summary>
    public class InputManager {
       
        #region  Singleton declaration
        private static InputManager _Instance;
        //curInstance ??= new(); //compound assignment, equivalent to checkin if its null then assigning it
        public static InputManager Instance => _Instance ??= new();
        #endregion
        
        public InputManager() {
            InputActions = new InputActions();
        }

        public InputActions InputActions {get; protected set;}
    }

}

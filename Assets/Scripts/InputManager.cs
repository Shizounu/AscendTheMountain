using Shizounu.Library;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Input {
    /// <summary>
    /// Singleton class holding a reference to the global class 'input actions'
    /// </summary>
    public class InputManager : SingletonBehaviour<InputManager> {

        protected override void Awake() {
            base.Awake();

            InputActions = new();
        }
        public InputActions InputActions {get; protected set;}

        private void OnEnable()
        {
            InputActions.Enable();
        }
        private void OnDisable()
        {
            InputActions.Disable();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Shizounu.Library.ScriptableArchitecture;
using Input;
using System.ComponentModel;


namespace Map {
    public class CameraController : MonoBehaviour
    {
        [Header("Properties")]
        [Tooltip("Reference to scriptabel variable carrying height of the map")]
        public FloatReference mapHeight;
        [Tooltip("Speed the camera moves in, measured in units per second")]
        public float cameraSpeed = 3.5f;


        [Header("Reference Values")]
        private float moveDirection;
        private InputActions InputActions => InputManager.Instance.InputActions;

        private void Awake() {
            InputActions.MapControls.MoveCamera.performed += ctx => {
                moveDirection = ctx.ReadValue<float>();
            };
            InputActions.MapControls.MoveCamera.canceled += ctx => {
                moveDirection = 0;
            };
        }

        private void LateUpdate() {
            MoveCamera(moveDirection);
        }

        private void MoveCamera(float dir){
            float movement = cameraSpeed * dir * Time.deltaTime;

            transform.position += new Vector3(0, movement, 0);

            //clamps position
            transform.position = new Vector3(
                transform.position.x, 
                Mathf.Max(
                    Mathf.Min(transform.position.y, mapHeight / 2), 
                    -mapHeight / 2), 
                transform.position.z);
        }

        private void OnEnable() {
            InputActions.MapControls.MoveCamera.Enable();
        }
        private void OnDisable() {
            InputActions.MapControls.MoveCamera.Disable();
        }
    }
}

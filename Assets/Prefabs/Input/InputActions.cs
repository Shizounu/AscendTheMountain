//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.0
//     from Assets/Prefabs/Input/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Input
{
    public partial class @InputActions: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""General Controls"",
            ""id"": ""775ec2b0-db0d-4234-a6cd-4343c9be6f27"",
            ""actions"": [
                {
                    ""name"": ""Open Settings"",
                    ""type"": ""Button"",
                    ""id"": ""0f7bb646-95bd-493b-ad3e-9907993e519f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4e321777-61fc-4458-a6db-1bf6cb0b93d1"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""Open Settings"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Battlefield Controls"",
            ""id"": ""42d51438-61fc-49d7-82a3-6d561588209b"",
            ""actions"": [
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""3025704b-2361-42f4-bdf9-95598efae154"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""Button"",
                    ""id"": ""0a705f67-c755-4181-b2fc-33aa09e3c1f0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""215c0ce5-291f-4546-8a65-ca9df75bfadc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3c25eefc-70af-4686-a7e8-0c1ce2d5ea71"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ab1cf67-a6a8-4830-9a4a-2e9c527aaa4e"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85b41d44-e389-4d92-9685-884cb2d1f017"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Map Controls"",
            ""id"": ""dee299c7-ffa9-447b-9b5f-bb42f0dc82fa"",
            ""actions"": [
                {
                    ""name"": ""MoveCamera"",
                    ""type"": ""Value"",
                    ""id"": ""c9197ecd-ac70-4125-b376-d471b7d06eec"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""Normalize(min=-1,max=1)"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard WS"",
                    ""id"": ""a751df54-42c1-44c1-a1c3-d1db6b497d5b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d8c1b6ce-7384-4f6b-8dad-b7dcc22d7779"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""33eb456a-dbe8-4143-97e5-8195c3900604"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard Arrows"",
                    ""id"": ""9e87604e-311e-438d-bfe4-3e9a3de3f2b5"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""213c121e-8ec1-4a4f-ab6f-bbb0bab79644"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""7e3c2ac9-6075-4fdc-88fa-7825ce959bcf"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Mouse Scrolling"",
                    ""id"": ""278e5bcf-773e-4dec-acae-beef35b4cb85"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0.1)"",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""241197a2-8d91-46bf-8df6-aec4f0bf5a5e"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""7d42838b-93ca-4273-8c3c-7d5f0cbc2414"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse and Keyboard"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse and Keyboard"",
            ""bindingGroup"": ""Mouse and Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // General Controls
            m_GeneralControls = asset.FindActionMap("General Controls", throwIfNotFound: true);
            m_GeneralControls_OpenSettings = m_GeneralControls.FindAction("Open Settings", throwIfNotFound: true);
            // Battlefield Controls
            m_BattlefieldControls = asset.FindActionMap("Battlefield Controls", throwIfNotFound: true);
            m_BattlefieldControls_LeftClick = m_BattlefieldControls.FindAction("LeftClick", throwIfNotFound: true);
            m_BattlefieldControls_RightClick = m_BattlefieldControls.FindAction("RightClick", throwIfNotFound: true);
            m_BattlefieldControls_MousePosition = m_BattlefieldControls.FindAction("MousePosition", throwIfNotFound: true);
            // Map Controls
            m_MapControls = asset.FindActionMap("Map Controls", throwIfNotFound: true);
            m_MapControls_MoveCamera = m_MapControls.FindAction("MoveCamera", throwIfNotFound: true);
        }

        ~@InputActions()
        {
            UnityEngine.Debug.Assert(!m_GeneralControls.enabled, "This will cause a leak and performance issues, InputActions.GeneralControls.Disable() has not been called.");
            UnityEngine.Debug.Assert(!m_BattlefieldControls.enabled, "This will cause a leak and performance issues, InputActions.BattlefieldControls.Disable() has not been called.");
            UnityEngine.Debug.Assert(!m_MapControls.enabled, "This will cause a leak and performance issues, InputActions.MapControls.Disable() has not been called.");
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // General Controls
        private readonly InputActionMap m_GeneralControls;
        private List<IGeneralControlsActions> m_GeneralControlsActionsCallbackInterfaces = new List<IGeneralControlsActions>();
        private readonly InputAction m_GeneralControls_OpenSettings;
        public struct GeneralControlsActions
        {
            private @InputActions m_Wrapper;
            public GeneralControlsActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @OpenSettings => m_Wrapper.m_GeneralControls_OpenSettings;
            public InputActionMap Get() { return m_Wrapper.m_GeneralControls; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GeneralControlsActions set) { return set.Get(); }
            public void AddCallbacks(IGeneralControlsActions instance)
            {
                if (instance == null || m_Wrapper.m_GeneralControlsActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_GeneralControlsActionsCallbackInterfaces.Add(instance);
                @OpenSettings.started += instance.OnOpenSettings;
                @OpenSettings.performed += instance.OnOpenSettings;
                @OpenSettings.canceled += instance.OnOpenSettings;
            }

            private void UnregisterCallbacks(IGeneralControlsActions instance)
            {
                @OpenSettings.started -= instance.OnOpenSettings;
                @OpenSettings.performed -= instance.OnOpenSettings;
                @OpenSettings.canceled -= instance.OnOpenSettings;
            }

            public void RemoveCallbacks(IGeneralControlsActions instance)
            {
                if (m_Wrapper.m_GeneralControlsActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IGeneralControlsActions instance)
            {
                foreach (var item in m_Wrapper.m_GeneralControlsActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_GeneralControlsActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public GeneralControlsActions @GeneralControls => new GeneralControlsActions(this);

        // Battlefield Controls
        private readonly InputActionMap m_BattlefieldControls;
        private List<IBattlefieldControlsActions> m_BattlefieldControlsActionsCallbackInterfaces = new List<IBattlefieldControlsActions>();
        private readonly InputAction m_BattlefieldControls_LeftClick;
        private readonly InputAction m_BattlefieldControls_RightClick;
        private readonly InputAction m_BattlefieldControls_MousePosition;
        public struct BattlefieldControlsActions
        {
            private @InputActions m_Wrapper;
            public BattlefieldControlsActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @LeftClick => m_Wrapper.m_BattlefieldControls_LeftClick;
            public InputAction @RightClick => m_Wrapper.m_BattlefieldControls_RightClick;
            public InputAction @MousePosition => m_Wrapper.m_BattlefieldControls_MousePosition;
            public InputActionMap Get() { return m_Wrapper.m_BattlefieldControls; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(BattlefieldControlsActions set) { return set.Get(); }
            public void AddCallbacks(IBattlefieldControlsActions instance)
            {
                if (instance == null || m_Wrapper.m_BattlefieldControlsActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_BattlefieldControlsActionsCallbackInterfaces.Add(instance);
                @LeftClick.started += instance.OnLeftClick;
                @LeftClick.performed += instance.OnLeftClick;
                @LeftClick.canceled += instance.OnLeftClick;
                @RightClick.started += instance.OnRightClick;
                @RightClick.performed += instance.OnRightClick;
                @RightClick.canceled += instance.OnRightClick;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }

            private void UnregisterCallbacks(IBattlefieldControlsActions instance)
            {
                @LeftClick.started -= instance.OnLeftClick;
                @LeftClick.performed -= instance.OnLeftClick;
                @LeftClick.canceled -= instance.OnLeftClick;
                @RightClick.started -= instance.OnRightClick;
                @RightClick.performed -= instance.OnRightClick;
                @RightClick.canceled -= instance.OnRightClick;
                @MousePosition.started -= instance.OnMousePosition;
                @MousePosition.performed -= instance.OnMousePosition;
                @MousePosition.canceled -= instance.OnMousePosition;
            }

            public void RemoveCallbacks(IBattlefieldControlsActions instance)
            {
                if (m_Wrapper.m_BattlefieldControlsActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IBattlefieldControlsActions instance)
            {
                foreach (var item in m_Wrapper.m_BattlefieldControlsActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_BattlefieldControlsActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public BattlefieldControlsActions @BattlefieldControls => new BattlefieldControlsActions(this);

        // Map Controls
        private readonly InputActionMap m_MapControls;
        private List<IMapControlsActions> m_MapControlsActionsCallbackInterfaces = new List<IMapControlsActions>();
        private readonly InputAction m_MapControls_MoveCamera;
        public struct MapControlsActions
        {
            private @InputActions m_Wrapper;
            public MapControlsActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @MoveCamera => m_Wrapper.m_MapControls_MoveCamera;
            public InputActionMap Get() { return m_Wrapper.m_MapControls; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MapControlsActions set) { return set.Get(); }
            public void AddCallbacks(IMapControlsActions instance)
            {
                if (instance == null || m_Wrapper.m_MapControlsActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_MapControlsActionsCallbackInterfaces.Add(instance);
                @MoveCamera.started += instance.OnMoveCamera;
                @MoveCamera.performed += instance.OnMoveCamera;
                @MoveCamera.canceled += instance.OnMoveCamera;
            }

            private void UnregisterCallbacks(IMapControlsActions instance)
            {
                @MoveCamera.started -= instance.OnMoveCamera;
                @MoveCamera.performed -= instance.OnMoveCamera;
                @MoveCamera.canceled -= instance.OnMoveCamera;
            }

            public void RemoveCallbacks(IMapControlsActions instance)
            {
                if (m_Wrapper.m_MapControlsActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IMapControlsActions instance)
            {
                foreach (var item in m_Wrapper.m_MapControlsActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_MapControlsActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public MapControlsActions @MapControls => new MapControlsActions(this);
        private int m_MouseandKeyboardSchemeIndex = -1;
        public InputControlScheme MouseandKeyboardScheme
        {
            get
            {
                if (m_MouseandKeyboardSchemeIndex == -1) m_MouseandKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse and Keyboard");
                return asset.controlSchemes[m_MouseandKeyboardSchemeIndex];
            }
        }
        public interface IGeneralControlsActions
        {
            void OnOpenSettings(InputAction.CallbackContext context);
        }
        public interface IBattlefieldControlsActions
        {
            void OnLeftClick(InputAction.CallbackContext context);
            void OnRightClick(InputAction.CallbackContext context);
            void OnMousePosition(InputAction.CallbackContext context);
        }
        public interface IMapControlsActions
        {
            void OnMoveCamera(InputAction.CallbackContext context);
        }
    }
}

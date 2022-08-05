using System;
using System.Linq;
using Input.Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Input
{
    public class InputService : IInputService, IDisposable
    {
        private const string DeviceManufacturerNameDualshock = "Sony Interactive Entertainment";

        public Subject<ControllerType> DeviceChanged => deviceChanged;

        public Subject<Vector2> MoveVectorChanged => moveVectorChanged;

        private readonly Subject<ControllerType> deviceChanged = new Subject<ControllerType>();
        private readonly Subject<Vector2> moveVectorChanged = new Subject<Vector2>(); 
        
        private ControllerType currentController;
        private PlayerInput playerInput;
        private InputActionAsset actions;
        
        
        [Inject]
        private InputService(PlayerInput playerInput, InputActionAsset actions)
        {
            this.playerInput = playerInput;
            this.actions = actions;
            actions.FindAction("Move").performed += OnMovePerformed;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            OnActionPerformed();
            moveVectorChanged.OnNext(context.ReadValue<Vector2>());
        }
        
        private void OnActionPerformed()
        {
            ControllerType performedWithController;
            switch (playerInput.currentControlScheme)
            {
                case "Keyboard&Mouse":
                    performedWithController = ControllerType.Keyboard;
                    break;
                case "Gamepad":
                    if (InputSystem.devices.FirstOrDefault(d => d.description.manufacturer == DeviceManufacturerNameDualshock) != null)
                        performedWithController = ControllerType.Dualshock;
                    else
                        performedWithController = ControllerType.XboxController;

                    break;
                default:
                    performedWithController = ControllerType.Keyboard;
                    break;
                
            }

            if (performedWithController == currentController) return;
            
            currentController = performedWithController;
            deviceChanged.OnNext(currentController);
        }

        public void Dispose()
        {
            actions.FindAction("Move").performed -= OnMovePerformed;
        }
    }
}

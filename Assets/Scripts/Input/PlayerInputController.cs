using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class PlayerInputController : MonoBehaviour
    {
        public static PlayerInputController Instance;
        
        private PlayerInputAction _playerInputAction;
        public event EventHandler OnInteractAction;
        public event EventHandler OnInteractAlternateAction;
        public event EventHandler OnPauseInteraction;

        private void Awake()
        {
            Instance = this;
            _playerInputAction = new PlayerInputAction();
            _playerInputAction.PlayerMove.Enable();
            _playerInputAction.PlayerMove.Interact.performed += InteractPerformed;
            _playerInputAction.PlayerMove.InteractAlternate.performed += InteractAlternatePerformed;
            _playerInputAction.PlayerMove.Pause.performed += PausePerformed;
        }

        private void OnDestroy()
        {
            _playerInputAction.PlayerMove.Interact.performed -= InteractPerformed;
            _playerInputAction.PlayerMove.InteractAlternate.performed -= InteractAlternatePerformed;
            _playerInputAction.PlayerMove.Pause.performed -= PausePerformed;
            
            _playerInputAction.Dispose();
        }

        private void PausePerformed(InputAction.CallbackContext obj)
        {
            OnPauseInteraction?.Invoke(this,EventArgs.Empty);
        }

        private void InteractAlternatePerformed(InputAction.CallbackContext ctx)
        {
            OnInteractAlternateAction?.Invoke(this,EventArgs.Empty);
        }

        private void InteractPerformed(InputAction.CallbackContext ctx)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }

        public Vector2 GetMovementVectorNormalized()
        {
            return  _playerInputAction.PlayerMove.Move.ReadValue<Vector2>().normalized;
        }
    }
}

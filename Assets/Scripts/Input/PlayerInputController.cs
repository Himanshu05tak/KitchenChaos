using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class PlayerInputController : MonoBehaviour
    {
        private PlayerInputAction _playerInputAction;
        public event EventHandler OnInteractAction;
        public event EventHandler OnInteractAlternateAction;

        private void Awake()
        {
            _playerInputAction = new PlayerInputAction();
            _playerInputAction.PlayerMove.Enable();
            _playerInputAction.PlayerMove.Interact.performed += InteractPerformed;
            _playerInputAction.PlayerMove.InteractAlternate.performed += InteractAlternateOnPerformed;
        }

        private void InteractAlternateOnPerformed(InputAction.CallbackContext ctx)
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

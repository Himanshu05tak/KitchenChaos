using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class PlayerInputController : MonoBehaviour
    {
        private PlayerInputAction _playerInputAction;
        public event EventHandler OnInteractAction;
        private void Awake()
        {
            _playerInputAction = new PlayerInputAction();
            _playerInputAction.PlayerMove.Enable();
            _playerInputAction.PlayerMove.Interact.performed += InteractPerformed;
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

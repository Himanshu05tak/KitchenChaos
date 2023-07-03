using UnityEngine;

namespace Input
{
    public class PlayerInputController : MonoBehaviour
    {
        private PlayerInputAction _playerInputAction;
        private void Awake()
        {
            _playerInputAction = new PlayerInputAction();
            _playerInputAction.PlayerMove.Enable();
        }
        public Vector2 GetMovementVectorNormalized()
        {
            return  _playerInputAction.PlayerMove.Move.ReadValue<Vector2>().normalized;
        }
    }
}

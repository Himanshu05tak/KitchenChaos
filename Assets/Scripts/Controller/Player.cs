using Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float smoothRotation;
        [SerializeField] private PlayerInputController playerInputController;

        private Transform _transform;
        private bool _isWalking;
        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            PlayerMovement();
        }

        private void PlayerMovement()
        {
            Vector3 moveDir = playerInputController.GetMovementVectorNormalized();
            var moveInputDirection = new Vector3(moveDir.x, 0, moveDir.y);
            _transform.position += moveInputDirection * (speed * Time.deltaTime);
            _isWalking = moveInputDirection != Vector3.zero;
            _transform.forward = Vector3.Slerp(transform.forward,moveInputDirection,smoothRotation*Time.deltaTime);
        }

        public bool IsWalking()
        {
            return _isWalking;
        }
    }
}

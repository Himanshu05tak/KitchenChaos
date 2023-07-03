using Input;
using UnityEngine;

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
            Vector3 inputVectorNormalized = playerInputController.GetMovementVectorNormalized();
            var moveDir = new Vector3(inputVectorNormalized.x, 0, inputVectorNormalized.y);
            var moveDistance = speed * Time.deltaTime;
         
            if (!CanMove(moveDir, moveDistance))
            {
                var moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
                CanMove(moveDirX, moveDistance);
                if (CanMove(moveDirX, moveDistance))
                    moveDir = moveDirX;
                else
                {
                    var moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                    CanMove(moveDirZ, moveDistance);
                    if (CanMove(moveDirZ, moveDistance))
                        moveDir = moveDirZ;
                }
            }
            if(CanMove(moveDir,moveDistance))
                _transform.position += moveDir * moveDistance;
            _isWalking = moveDir != Vector3.zero;
            _transform.forward = Vector3.Slerp(transform.forward,moveDir,smoothRotation*Time.deltaTime);
        }

        private bool CanMove(Vector3 moveDir,float moveDistance)
        {
            const int playerHeight = 2;
            const float playerRadius = .7f;
            return !Physics.CapsuleCast(_transform.position, transform.position + Vector3.up * playerHeight,
                playerRadius, moveDir, moveDistance);
        }

        public bool IsWalking()
        {
            return _isWalking;
        }
    }
}

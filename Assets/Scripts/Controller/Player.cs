using UnityEngine;

namespace Controller
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float smoothRotation;

        private Transform _transform;
        private bool _isWalking;
        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            PlayerInput();
        }

        private void PlayerInput()
        {
            var inputVector = new Vector2(0, 0);
            if (Input.GetKey(KeyCode.W))
            {
                inputVector.y += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputVector.x -= 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputVector.y -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputVector.x += 1;
            }

            var moveDir = new Vector3(inputVector.x,0,inputVector.y).normalized;
            _transform.position += moveDir * (speed * Time.deltaTime);
            _isWalking = moveDir != Vector3.zero;
            _transform.forward = Vector3.Slerp(transform.forward,moveDir,smoothRotation*Time.deltaTime);
        }

        public bool IsWalking()
        {
            return _isWalking;
        }
    }
}

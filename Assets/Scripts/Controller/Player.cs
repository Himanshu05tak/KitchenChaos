using Input;
using System;
using Manager;
using Counters.KitchenCounters;
using Interface;
using Unity.Netcode;
using UnityEngine;

namespace Controller
{
    public class Player : NetworkBehaviour, IKitchenObjectParent
    {
        //public static Player Instance { get; private set; }
        public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterCharged;

        public event EventHandler OnPickSomething;
        public class OnSelectedCounterChangedEventArgs : EventArgs
        {
            public BaseCounter SelectedCounter;
        }

        [SerializeField] private float speed;
        [SerializeField] private float smoothRotation;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Transform kitchenObjectHoldPoint;


        private Transform _transform;
        private bool _isWalking;
        private Vector3 _lastInteraction;
        private BaseCounter _selectedCounter;
        private KitchenObject.KitchenObject _kitchenObject;
        
        private void Awake()
        {
            // if(Instance!=null)
            //     Debug.LogError("There is more than one player instance");
            // Instance = this;
            _transform = GetComponent<Transform>();
        }

        private void Start()
        {
            PlayerInputController.Instance.OnInteractAction += PlayerInputControllerOnOnInteractAction;
            PlayerInputController.Instance.OnInteractAlternateAction += PlayerInputControllerOnOnInteractAlternateAction;
        }

        private void PlayerInputControllerOnOnInteractAlternateAction(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsGamePlaying()) return;
            if (_selectedCounter != null)
            {
                _selectedCounter.InteractAlternate(this);
            }
        }

        private void PlayerInputControllerOnOnInteractAction(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsGamePlaying()) return;
            if (_selectedCounter != null)
            {
                _selectedCounter.Interact(this);
            }
        }

        private void Update()
        {
            if(!IsOwner) return;
            HandleMovement();
            HandleInteraction();
        }

        private void HandleMovement()
        {
            Vector3 inputVectorNormalized = PlayerInputController.Instance.GetMovementVectorNormalized();
            var moveDir = new Vector3(inputVectorNormalized.x, 0, inputVectorNormalized.y);
            var moveDistance = speed * Time.deltaTime;
         
            if (!CanMove(moveDir, moveDistance))
            {
                //Cannot move towards moveDir
                
                //Attempt only X movement
                var moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
                CanMove(moveDirX, moveDistance);

                if (moveDir.x is < -.5f or > .5f && CanMove(moveDirX, moveDistance)) 
                    //Can move only on the X
                    moveDir = moveDirX;
                else
                {
                    //Cannot move only on the X
                    
                    //Attempt only Z movement
                    var moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                    CanMove(moveDirZ, moveDistance);
                    if (moveDir.z is < -.5f or > .5f && CanMove(moveDirZ, moveDistance))
                        //Can move only on the Z
                        moveDir = moveDirZ;
                    //Cannot move in any direction
                }
            }
            if(CanMove(moveDir,moveDistance))
                _transform.position += moveDir * moveDistance;
            _isWalking = moveDir != Vector3.zero;
            _transform.forward = Vector3.Slerp(transform.forward,moveDir,smoothRotation*Time.deltaTime);
        }

        private void HandleInteraction()
        {
            Vector3 inputVectorNormalized = PlayerInputController.Instance.GetMovementVectorNormalized();
            var moveDir = new Vector3(inputVectorNormalized.x, 0, inputVectorNormalized.y);
            
            const float interactionDistance = 2f;

            if (moveDir != Vector3.zero)
                _lastInteraction = moveDir;
            var isInteract = Physics.Raycast(transform.position, _lastInteraction, out var hitInfo, interactionDistance,layerMask);

            if (isInteract && hitInfo.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != null && baseCounter != _selectedCounter)
                    _selectedCounter = baseCounter;
                SetSelectedCounter(_selectedCounter);
            }
            else
            {
                SetSelectedCounter(null);
            }
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

        private void SetSelectedCounter(BaseCounter selectedCounter)
        {
            _selectedCounter = selectedCounter;
            OnSelectedCounterCharged?.Invoke(this,new OnSelectedCounterChangedEventArgs {SelectedCounter = _selectedCounter});
        }

        public Transform GetKitchenObjectFollowTransform()
        {
            return kitchenObjectHoldPoint;
        }
        public void SetKitchenObject(KitchenObject.KitchenObject kitchenObject)
        {
            _kitchenObject = kitchenObject;
            if(kitchenObject!=null)
                OnPickSomething?.Invoke(this,EventArgs.Empty);
        }

        public KitchenObject.KitchenObject GetKitchenObject()
        {
            return _kitchenObject;
        }

        public void ClearKitchenObject()
        {
            _kitchenObject = null;
        }

        public bool HasKitchenObject()
        {
            return _kitchenObject!=null;
        }
    }
}

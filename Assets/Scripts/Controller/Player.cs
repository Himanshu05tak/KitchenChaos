using Input;
using System;
using Manager;
using Interface;
using UnityEngine;
using Unity.Netcode;
using Counters.KitchenCounters;
using System.Collections.Generic;

namespace Controller
{
    public class Player : NetworkBehaviour, IKitchenObjectParent
    {
        public static Player LocalInstance { get; private set; }
        public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterCharged;

        public static event EventHandler OnAnyPlayerSpawned;
        public static event EventHandler OnAnyPickedSomething;
        public static void ResetStaticData()
        {
            OnAnyPlayerSpawned = null;
        }
        public event EventHandler OnPickSomething;
        public class OnSelectedCounterChangedEventArgs : EventArgs
        {
            public BaseCounter SelectedCounter;
        }

        [SerializeField] private float speed;
        [SerializeField] private float smoothRotation;
        [SerializeField] private LayerMask counterLayerMask;
        [SerializeField] private LayerMask collisionLayerMask;
        [SerializeField] private Transform kitchenObjectHoldPoint;
        [SerializeField] private List<Vector3> spawnPositionList;

        
        private bool _isWalking;
        private Vector3 _lastInteraction;
        private BaseCounter _selectedCounter;
        private KitchenObject.KitchenObject _kitchenObject;
        
        private void Start()
        {
            PlayerInputController.Instance.OnInteractAction += PlayerInputControllerOnOnInteractAction;
            PlayerInputController.Instance.OnInteractAlternateAction += PlayerInputControllerOnOnInteractAlternateAction;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner) LocalInstance = this;

            transform.position = spawnPositionList[(int)OwnerClientId];
            OnAnyPlayerSpawned?.Invoke(this,EventArgs.Empty);
            
            if(IsServer)
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }

        private void OnClientDisconnectCallback(ulong clientID)
        {
            if (clientID == OwnerClientId && HasKitchenObject())
            {
                KitchenObject.KitchenObject.DestroyKitchenObject(GetKitchenObject());
            }
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
                transform.position += moveDir * moveDistance;
            _isWalking = moveDir != Vector3.zero;
            transform.forward = Vector3.Slerp(transform.forward,moveDir,smoothRotation*Time.deltaTime);
        }

        private void HandleInteraction()
        {
            Vector3 inputVectorNormalized = PlayerInputController.Instance.GetMovementVectorNormalized();
            var moveDir = new Vector3(inputVectorNormalized.x, 0, inputVectorNormalized.y);
            
            const float interactionDistance = 2f;

            if (moveDir != Vector3.zero)
                _lastInteraction = moveDir;
            var isInteract = Physics.Raycast(transform.position, _lastInteraction, out var hitInfo, interactionDistance,counterLayerMask);

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
            return !Physics.BoxCast(transform.position, Vector3.one * playerRadius,
                moveDir, Quaternion.identity, moveDistance, collisionLayerMask);
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
            if (kitchenObject == null) return;
            OnPickSomething?.Invoke(this,EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this,EventArgs.Empty);
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

        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }
    }
}

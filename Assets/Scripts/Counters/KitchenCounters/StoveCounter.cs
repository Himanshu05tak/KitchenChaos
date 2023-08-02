using System;
using System.Linq;
using Controller;
using Interface;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counters.KitchenCounters
{
    public class StoveCounter : BaseCounter, IHasProgress
    {
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
        public class OnStateChangedEventArgs : EventArgs
        {
            public FryingState FryingState;
        }
        public enum FryingState
        {
            Idle,
            Frying,
            Fried,
            Burned,
        }

        [SerializeField] private FryingRecipeSO[] fryingRecipeSoArray;
        [SerializeField] private BurningRecipeSO[] burningRecipeSoArray;
        
        private readonly NetworkVariable<FryingState> _fryingState = new();
        private readonly NetworkVariable<float> _fryingTimer = new ();
        private readonly NetworkVariable<float> _burningTimer = new ();

        private FryingRecipeSO _fryingRecipeSo;
        private BurningRecipeSO _burningRecipeSo;
        public override void OnNetworkSpawn()
        {
            _fryingTimer.OnValueChanged += FryingTimerOnValueChanged;
            _burningTimer.OnValueChanged += BurningTimerOnValueChanged;
            _fryingState.OnValueChanged += FryingStateOnValueChanged;
        }

        private void FryingStateOnValueChanged(FryingState previousValue, FryingState newValue)
        {
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() { FryingState = _fryingState.Value });
            if (_fryingState.Value is FryingState.Burned or FryingState.Idle) 
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { ProgressNormalized = 0f });
        }

        private void BurningTimerOnValueChanged(float previousValue, float newValue)
        {
            var burningTimerMax = _burningRecipeSo != null ? _burningRecipeSo.burningTimerMax : 1;
            OnProgressChanged?.Invoke(this,
                new IHasProgress.OnProgressChangedEventArgs()
                    { ProgressNormalized = _burningTimer.Value / burningTimerMax });
        }

        private void FryingTimerOnValueChanged(float previousValue, float newValue)
        {
            var fryingTimerMax = _fryingRecipeSo != null ? _fryingRecipeSo.fryingTimerMax : 1;
            OnProgressChanged?.Invoke(this,
                new IHasProgress.OnProgressChangedEventArgs()
                    { ProgressNormalized = _fryingTimer.Value / fryingTimerMax });
        }

        private void Update()
        {
            if(!IsServer) return;
            if (!HasKitchenObject()) return;
            switch (_fryingState.Value)
            {
                case FryingState.Idle:
                    break;
                case FryingState.Frying:
                    _fryingTimer.Value += Time.deltaTime;
                  
                    if (_fryingTimer.Value > _fryingRecipeSo.fryingTimerMax)
                    {
                        KitchenObject.KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);
                        
                        _fryingState.Value = FryingState.Fried;
                        _burningTimer.Value = 0f;
                        SetBurningRecipeSoClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSoIndex(GetKitchenObject().GetKitchenObjectSo));
                    } 
                    break;
                case FryingState.Fried:
                    _burningTimer.Value += Time.deltaTime;
                    
                    if (_burningTimer.Value > _burningRecipeSo.burningTimerMax)
                    {
                        //Fried
                        KitchenObject.KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        
                        KitchenObject.KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);
                        
                        _fryingState.Value = FryingState.Burned;
                    }
                    break;
                case FryingState.Burned:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Interact(Player player)
        {
            if (!HasKitchenObject())
            {
                //There is no kitchen Object here
                if (!player.HasKitchenObject()) return;
                //Player is carrying something 
                if (!HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSo)) return;
                //player carrying something that can be fried
                var kitchenObject =  player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);
                
                InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSoIndex(kitchenObject.GetKitchenObjectSo));
                //Player is not carrying anything
            }
            else
            {
                //This is kitchen Object here
                if (player.HasKitchenObject())
                {
                    //Player is carrying something 
                    if (!player.GetKitchenObject().TryGetPlate(out var plateKitchenObject)) return;
                    //Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSo))
                        KitchenObject.KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    
                    SetStateIdleServerRpc();
                }
                else
                {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    //Player is not carrying anything
                    SetStateIdleServerRpc();
                }
                //There is a kitchen Object here
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetStateIdleServerRpc()
        {
            _fryingState.Value = FryingState.Idle;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSoIndex)
        {
            _fryingTimer.Value = 0;
            _fryingState.Value = FryingState.Frying;

            SetFryingRecipeSoClientRpc(kitchenObjectSoIndex);
        }
        
        [ClientRpc]
        private void SetFryingRecipeSoClientRpc(int kitchenObjectSoIndex)
        {
            var kitchenObjectSo = KitchenGameMultiplayer.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
            _fryingRecipeSo = GetFryingRecipeSoWithInput(kitchenObjectSo);
        }

        [ClientRpc]
        private void SetBurningRecipeSoClientRpc(int kitchenObjectSoIndex)
        {
            var kitchenObjectSo = KitchenGameMultiplayer.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
            _burningRecipeSo = GetBurningRecipeSoWithInput(kitchenObjectSo);
        }
        private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            var cuttingRecipeSo = GetFryingRecipeSoWithInput(inputKitchenObjectSo);
            return cuttingRecipeSo != null;
        }

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
        {
            var fryingRecipeSo = GetFryingRecipeSoWithInput(inputKitchenObjectSo);
            return fryingRecipeSo != null ? fryingRecipeSo.output : null;
        }
        private FryingRecipeSO GetFryingRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            return (from fryingRecipeSo in fryingRecipeSoArray where fryingRecipeSo.input == inputKitchenObjectSo select fryingRecipeSo).FirstOrDefault();
        }
        
        private BurningRecipeSO GetBurningRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            return (from burningRecipeSo in burningRecipeSoArray where burningRecipeSo.input == inputKitchenObjectSo select burningRecipeSo).FirstOrDefault();
        }

        public bool IsFried()
        {
            return _fryingState.Value == FryingState.Fried;
        }
    }
}

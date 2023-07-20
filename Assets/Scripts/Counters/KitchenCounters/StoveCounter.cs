using System;
using System.Linq;
using Controller;
using Interface;
using ScriptableObjects;
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

        [SerializeField] private FryingState fryingState;
        [SerializeField] private FryingRecipeSO[] fryingRecipeSoArray;
        [SerializeField] private BurningRecipeSO[] burningRecipeSoArray;
        
        private float _fryingTimer;
        private float _burningTimer;

        private FryingRecipeSO _fryingRecipeSo;
        private BurningRecipeSO _burningRecipeSo;

        private void Start()
        {
            fryingState = FryingState.Idle;
        }

        private void Update()
        {
            if (!HasKitchenObject()) return;
            switch (fryingState)
            {
                case FryingState.Idle:
                    break;
                case FryingState.Frying:
                    _fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this,
                        new IHasProgress.OnProgressChangedEventArgs()
                            { ProgressNormalized = _fryingTimer / _fryingRecipeSo.fryingTimerMax });
                    if (_fryingTimer > _fryingRecipeSo.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.KitchenObject.SpawnKitchenObject(_fryingRecipeSo.output, this);
                        fryingState = FryingState.Fried;
                        _burningTimer = 0f;
                        _burningRecipeSo = GetBurningRecipeSoWithInput(GetKitchenObject().GetKitchenObjectSo);
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() { FryingState = fryingState });
                    } 
                    break;
                case FryingState.Fried:
                    
                    _burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { ProgressNormalized = _burningTimer / _burningRecipeSo.burningTimerMax });
                    if (_burningTimer > _burningRecipeSo.burningTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.KitchenObject.SpawnKitchenObject(_burningRecipeSo.output, this);
                        fryingState = FryingState.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() { FryingState = fryingState });
                    
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { ProgressNormalized = 0f });
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
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _fryingRecipeSo = GetFryingRecipeSoWithInput(GetKitchenObject().GetKitchenObjectSo);
                fryingState = FryingState.Frying;

                _fryingTimer = 0;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { FryingState = fryingState });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { ProgressNormalized = _fryingTimer / _fryingRecipeSo.fryingTimerMax });
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
                        GetKitchenObject().DestroySelf();
                    fryingState = FryingState.Idle;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { FryingState = fryingState });
                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { ProgressNormalized = 0f });
                }
                else
                {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    //Player is not carrying anything
                    fryingState = FryingState.Idle;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { FryingState = fryingState });
                    
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { ProgressNormalized = 0f });
                }
                //There is a kitchen Object here
            }
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
            return fryingState == FryingState.Fried;
        }
    }
}

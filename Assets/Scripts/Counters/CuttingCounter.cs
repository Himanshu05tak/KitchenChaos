using System;
using System.Linq;
using Controller;
using Interface;
using ScriptableObjects;
using UnityEngine;

namespace Counters
{
    public class CuttingCounter : BaseCounter, IHasProgress
    {
        [SerializeField] private CuttingRecipeSO[] cuttingRecipeSos;
        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
        public event EventHandler OnCut;
    
        private int _cuttingProgress;
        private bool _isRecipeReady;

        public override void Interact(Player player)
        {
            if (!HasKitchenObject())
            {
                //There is no kitchen Object here
                if (player.HasKitchenObject())
                {
                    //Player is carrying something 
                    if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObject))
                        //player carrying something that can be cut
                        player.GetKitchenObject().SetKitchenObjectParent(this);
                    _cuttingProgress = 0;
                    _isRecipeReady = false;
                
                    var cuttingRecipeSo = GetCuttingRecipeSoWithInput(GetKitchenObject().GetKitchenObject);
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { ProgressNormalized = (float)_cuttingProgress/ cuttingRecipeSo.cuttingProgressMax});
                }
                else
                {
                    //Player is not carrying anything
                }
            }
            else
            {
                if (player.HasKitchenObject())
                {
                    //Player is carrying something 
                }
                else
                {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    //Player is not carrying anything
                }
                //There is a kitchen Object here
            }
        }

        public override void InteractAlternate(Player player)
        {
            //Todo while cutting Counter is empty if I try to cut an exceptional occured 
        
            if (!HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObject)) return;
            if(_isRecipeReady) return;
            //There is a kitchenObject here and can be cut off.
            _cuttingProgress++;
            OnCut?.Invoke(this,EventArgs.Empty);
        
            var cuttingRecipeSo = GetCuttingRecipeSoWithInput(GetKitchenObject().GetKitchenObject);
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { ProgressNormalized = (float)_cuttingProgress/ cuttingRecipeSo.cuttingProgressMax});

            if (_cuttingProgress < cuttingRecipeSo.cuttingProgressMax) return;
            var outputKitchenObjectSo = GetOutputForInput(GetKitchenObject().GetKitchenObject);
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSo, this);
            _isRecipeReady = true;
        }

        private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            var cuttingRecipeSo = GetCuttingRecipeSoWithInput(inputKitchenObjectSo);
            return cuttingRecipeSo != null;
        }

        private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSo)
        {
            var cuttingRecipeSo = GetCuttingRecipeSoWithInput(inputKitchenObjectSo);
            return cuttingRecipeSo != null ? cuttingRecipeSo.output : null;
        }

        private CuttingRecipeSO GetCuttingRecipeSoWithInput(KitchenObjectSO inputKitchenObjectSo)
        {
            return (from cuttingRecipeSo in cuttingRecipeSos where cuttingRecipeSo.input == inputKitchenObjectSo select cuttingRecipeSo).FirstOrDefault();
        }
    }
}

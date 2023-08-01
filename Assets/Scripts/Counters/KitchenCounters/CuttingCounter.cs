using System;
using System.Linq;
using Controller;
using Interface;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counters.KitchenCounters
{
    public class CuttingCounter : BaseCounter, IHasProgress
    {
        [SerializeField] private CuttingRecipeSO[] cuttingRecipeSos;

        public static event EventHandler OnAnyCut;
        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
        public event EventHandler OnCut;
    
        private int _cuttingProgress;

        public override void Interact(Player player)
        {
            if (!HasKitchenObject())
            {
                //There is no kitchen Object here
                if (player.HasKitchenObject())
                {
                    //Player is carrying something 
                    if (!HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSo)) return;
                        //player carrying something that can be cut
                        var kitchenObject = player.GetKitchenObject();
                        kitchenObject.SetKitchenObjectParent(this);
                        InteractLogicPlaceObjectOnCounterServerRpc();
                }
                else
                {
                    //Player is not carrying anything
                }
            }
            else
            {
                //This is kitchen object here
                if (player.HasKitchenObject())
                {
                    //Player is carrying something 
                    if (!player.GetKitchenObject().TryGetPlate(out var plateKitchenObject)) return;
                    //Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSo))
                        GetKitchenObject().DestroySelf();
                }
                else
                {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    //Player is not carrying anything
                }
                //There is a kitchen Object here
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicPlaceObjectOnCounterServerRpc()
        {
            InteractLogicPlaceObjectOnCounterClientRpc();
        }
        
        [ClientRpc]
        private void InteractLogicPlaceObjectOnCounterClientRpc()
        {
            _cuttingProgress = 0;
           
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { ProgressNormalized = 0f});
        }

        public override void InteractAlternate(Player player)
        {
            if (!HasKitchenObject()) return;
            if (!HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSo)) return;
            //There is a kitchenObject here and can be cut off.
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void CutObjectServerRpc()
        {
            CutObjectClientRpc();
        }

        [ClientRpc]
        private void CutObjectClientRpc()
        {
            _cuttingProgress++;

            OnCut?.Invoke(this,EventArgs.Empty);
            OnAnyCut?.Invoke(this,EventArgs.Empty);
            
            var cuttingRecipeSo = GetCuttingRecipeSoWithInput(GetKitchenObject().GetKitchenObjectSo);
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { ProgressNormalized = (float)_cuttingProgress/ cuttingRecipeSo.cuttingProgressMax});
        }

        [ServerRpc(RequireOwnership = false)]
        private void TestCuttingProgressDoneServerRpc()
        {
            var cuttingRecipeSo = GetCuttingRecipeSoWithInput(GetKitchenObject().GetKitchenObjectSo);
            if (_cuttingProgress < cuttingRecipeSo.cuttingProgressMax) return;
            var outputKitchenObjectSo = GetOutputForInput(GetKitchenObject().GetKitchenObjectSo);
            KitchenObject.KitchenObject.DestroyKitchenObject(GetKitchenObject());
            KitchenObject.KitchenObject.SpawnKitchenObject(outputKitchenObjectSo, this);
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
        public new static void ResetStaticData()
        {
            OnAnyCut = null;
        }
    }
}

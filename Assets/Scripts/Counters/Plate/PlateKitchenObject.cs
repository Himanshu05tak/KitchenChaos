using System;
using UnityEngine;
using ScriptableObjects;
using System.Collections.Generic;
using Unity.Netcode;

namespace Counters.Plate
{
    public class PlateKitchenObject : KitchenObject.KitchenObject
    {
        public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

        public class OnIngredientAddedEventArgs : EventArgs
        {
            public KitchenObjectSO KitchenObjectSo;
        }
        
        [SerializeField] private List<KitchenObjectSO> validKitchenObjectSos;
        private List<KitchenObjectSO> _kitchenObjectSos;

        protected override void Awake()
        {
            base.Awake();
            _kitchenObjectSos = new List<KitchenObjectSO>();
        }

        public bool TryAddIngredient(KitchenObjectSO kitchenObjectSo)
        {
            if (!validKitchenObjectSos.Contains(kitchenObjectSo)) return false;
            if (_kitchenObjectSos.Contains(kitchenObjectSo)) return false;
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSoIndex(kitchenObjectSo));
           
            return true;
        }
        public List<KitchenObjectSO> GetKitchenObjectSoList()
        {
            return _kitchenObjectSos;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void AddIngredientServerRpc(int kitchenObjectSoIndex)
        {
            AddIngredientClientRpc(kitchenObjectSoIndex);
        } 
        [ClientRpc]
        private void AddIngredientClientRpc(int kitchenObjectSoIndex)
        {
            var kitchenObjectSo = KitchenGameMultiplayer.Instance.GetKitchenObjectSoFromIndex(kitchenObjectSoIndex);
            _kitchenObjectSos.Add(kitchenObjectSo);
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { KitchenObjectSo = kitchenObjectSo });
        } 
    }
}
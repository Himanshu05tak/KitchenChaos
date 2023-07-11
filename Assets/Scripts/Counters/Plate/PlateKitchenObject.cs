using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

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

        private void Awake()
        {
            _kitchenObjectSos = new List<KitchenObjectSO>();
        }

        public bool TryAddIngredient(KitchenObjectSO kitchenObjectSo)
        {
            if (!validKitchenObjectSos.Contains(kitchenObjectSo)) return false;
            if (_kitchenObjectSos.Contains(kitchenObjectSo)) return false;
            _kitchenObjectSos.Add(kitchenObjectSo);
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { KitchenObjectSo = kitchenObjectSo });
            return true;
        }
    }
}
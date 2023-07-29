using System;
using UnityEngine;
using ScriptableObjects;
using System.Collections.Generic;

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
            _kitchenObjectSos.Add(kitchenObjectSo);
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { KitchenObjectSo = kitchenObjectSo });
            return true;
        }
        public List<KitchenObjectSO> GetKitchenObjectSoList()
        {
            return _kitchenObjectSos;
        }
    }
}
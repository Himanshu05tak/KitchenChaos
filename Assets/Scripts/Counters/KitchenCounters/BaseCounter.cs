using System;
using Controller;
using Interface;
using Unity.Netcode;
using UnityEngine;

namespace Counters.KitchenCounters
{
    public abstract class BaseCounter : NetworkBehaviour, IKitchenObjectParent
    {
        public static event EventHandler OnAnyObjectPlacedHere;
        public abstract void Interact(Player player);

        public virtual void InteractAlternate(Player player)
        {
            //Debug.LogError("BaseCounter.InteractAlternate()");   
        }
    
        [SerializeField] private Transform counterTopPoint;
    
        private KitchenObject.KitchenObject _kitchenObject;

        public Transform GetKitchenObjectFollowTransform()
        {
            return counterTopPoint;
        }
        public void SetKitchenObject(KitchenObject.KitchenObject kitchenObject)
        {
            _kitchenObject = kitchenObject;
            if (kitchenObject != null) 
                OnAnyObjectPlacedHere?.Invoke(this,EventArgs.Empty);
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

        public static void ResetStaticData()
        {
            OnAnyObjectPlacedHere = null;
        }
        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }
    }
}

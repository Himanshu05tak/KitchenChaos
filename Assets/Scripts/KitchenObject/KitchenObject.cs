using Interface;
using UnityEngine;
using Unity.Netcode;
using Counters.Plate;
using ScriptableObjects;

namespace KitchenObject
{
    public class KitchenObject : NetworkBehaviour
    {
        [SerializeField] private KitchenObjectSO kitchenObject;

        private IKitchenObjectParent _kitchenObjectParent;
        private FollowTransform _followTransform;
        public KitchenObjectSO GetKitchenObjectSo => kitchenObject;

        protected virtual void Awake()
        {
            _followTransform = GetComponent<FollowTransform>();
        }

        public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
        {
            SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
        {
            SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
        }

        [ClientRpc]
        private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
        {
            kitchenObjectParentNetworkObjectReference.TryGet(out var kitchenObjectParentNetworkObject);
            var kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
            
            _kitchenObjectParent?.ClearKitchenObject();
            _kitchenObjectParent = kitchenObjectParent;
     
            if (_kitchenObjectParent.HasKitchenObject())
                Debug.LogError("kitchenObjectParent already has a kitchenObject");   
            
            kitchenObjectParent.SetKitchenObject(this);
            _followTransform.SetTargetTransform(_kitchenObjectParent.GetKitchenObjectFollowTransform());
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public void ClearKitchenObjectParent()
        {
            _kitchenObjectParent.ClearKitchenObject();
        }

        public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
        {
            if (this is PlateKitchenObject)
            {
                plateKitchenObject = this as PlateKitchenObject;
                return true;
            }
            plateKitchenObject = null;
            return false;
        }

        public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent parent)
        {
            KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSo, parent);
        }
        
        public static void DestroyKitchenObject(KitchenObject kitchenObject)
        {
            KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
        }
        public IKitchenObjectParent GetKitchenObjectParent => _kitchenObjectParent;
    }
}

using UnityEngine;

namespace Interface
{
    public interface IKitchenObjectParent
    {
        public Transform GetKitchenObjectFollowTransform();

        public void SetKitchenObject(KitchenObject.KitchenObject kitchenObject);

        public KitchenObject.KitchenObject GetKitchenObject();

        public void ClearKitchenObject();

        public bool HasKitchenObject();
    }
}

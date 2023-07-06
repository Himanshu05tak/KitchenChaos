using Controller;
using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public abstract void Interact(Player player);

    public virtual void InteractAlternate(Player player)
    {
        Debug.LogError("BaseCounter.InteractAlternate()");   
    }
    
    [SerializeField] private Transform counterTopPoint;
    
    private KitchenObject _kitchenObject;

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
    }
    public KitchenObject GetKitchenObject()
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
}

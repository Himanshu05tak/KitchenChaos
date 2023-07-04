using Controller;
using ScriptableObjects;
using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO tomatoPrefab;
    [SerializeField] private Transform counterTopPoint;
    
    private KitchenObject _kitchenObject;
    
    public void Interact(Player player)
    {
        if (_kitchenObject == null)
        {
            var kitchenObjTransform = Instantiate(tomatoPrefab.prefab, counterTopPoint);
            kitchenObjTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }
        else
        {
            Debug.Log(_kitchenObject.GetKitchenObjectParent);
            _kitchenObject.SetKitchenObjectParent(player);
        }
    }

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
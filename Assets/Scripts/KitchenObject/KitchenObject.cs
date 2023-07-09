using ScriptableObjects;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSo;

    private IKitchenObjectParent _kitchenObjectParent;
    public KitchenObjectSO GetKitchenObject => kitchenObjectSo;

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        _kitchenObjectParent?.ClearKitchenObject();

        _kitchenObjectParent = kitchenObjectParent;
     
        if (_kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("kitchenObjectParent already has a kitchenObject");   
        }
        kitchenObjectParent.SetKitchenObject(this);
        
        transform.parent = _kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public void DestroySelf()
    {
        _kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSo, IKitchenObjectParent parent)
    {
        var kitchenObjTransform = Instantiate(kitchenObjectSo.prefab);
        var kitchenObject = kitchenObjTransform.GetComponent<KitchenObject>();
            kitchenObject.SetKitchenObjectParent(parent);
            return kitchenObject;
    }

    public IKitchenObjectParent GetKitchenObjectParent => _kitchenObjectParent;
}
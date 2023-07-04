using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSo;

    private IKitchenObjectParent _kitchenObjectParent;
    public KitchenObjectSO GetKitchenObject => kitchenObjectSo;

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if(_kitchenObjectParent!=null)
            _kitchenObjectParent.ClearKitchenObject();

        _kitchenObjectParent = kitchenObjectParent;
     
        if (_kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("kitchenObjectParent already has a kitchenObject");   
        }
        kitchenObjectParent.SetKitchenObject(this);
        
        transform.parent = _kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent => _kitchenObjectParent;
}

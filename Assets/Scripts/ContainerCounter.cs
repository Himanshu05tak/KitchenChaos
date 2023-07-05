using System;
using Controller;
using ScriptableObjects;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO tomatoPrefab;
    public event EventHandler OnPlayerGrabObject;
    public override void Interact(Player player)
    {
        var kitchenObjTransform = Instantiate(tomatoPrefab.prefab);
        kitchenObjTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);
        OnPlayerGrabObject?.Invoke(this,EventArgs.Empty);
    }
}

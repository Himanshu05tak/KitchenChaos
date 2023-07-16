using System;
using Controller;
using Counters;
using ScriptableObjects;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSo;
    public event EventHandler OnPlayerGrabObject;
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject()) return;
        //Player isn't carrying anything
        KitchenObject.KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);
        OnPlayerGrabObject?.Invoke(this,EventArgs.Empty);
    }
}

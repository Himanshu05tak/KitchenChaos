using Controller;
using ScriptableObjects;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO cutKitchenObjectSo;
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //There is no kitchen Object here
            Debug.Log("There is no kitchen Object ");
            if (player.HasKitchenObject())
            {
                //Player is carrying something 
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //Player is not carrying anything
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                //Player is carrying something 
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                //Player is not carrying anything
            }
            //There is a kitchen Object here
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (!HasKitchenObject()) return;
        GetKitchenObject().DestroySelf();

        KitchenObject.SpawnKitchenObject(cutKitchenObjectSo, this);
    }
}

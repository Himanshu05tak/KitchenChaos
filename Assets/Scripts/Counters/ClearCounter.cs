using Controller;
using ScriptableObjects;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO tomatoPrefab;
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //There is no kitchen Object here
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
}
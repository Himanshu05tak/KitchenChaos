using Controller;

namespace Counters.KitchenCounters
{
    public class ClearCounter : BaseCounter
    {
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
                //There is a kitchen object here
                if (player.HasKitchenObject())
                {
                    //Player is carrying something 
                    if (player.GetKitchenObject().TryGetPlate(out var plateKitchenObject))
                    {
                        //player is holding a plate
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSo))
                            GetKitchenObject().DestroySelf();
                    }
                    else
                    {
                        //player is not carrying plate but something else
                        if (!GetKitchenObject().TryGetPlate(out plateKitchenObject)) return;
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSo))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
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
}
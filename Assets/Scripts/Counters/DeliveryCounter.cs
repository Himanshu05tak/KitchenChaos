using Controller;

namespace Counters
{
    public class DeliveryCounter : BaseCounter
    {
        public override void Interact(Player player)
        {
            //Delivery
            if (!player.HasKitchenObject()) return;

            if (!player.GetKitchenObject().TryGetPlate(out var plateKitchenObject)) return;
            DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
            player.GetKitchenObject().DestroySelf();
        }
    }
}

using Controller;
using Counters.KitchenCounters;

namespace Counters.Deliver
{
    public class DeliveryCounter : BaseCounter
    {
        public static DeliveryCounter Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

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

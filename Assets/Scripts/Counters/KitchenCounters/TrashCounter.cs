using System;
using Controller;

namespace Counters.KitchenCounters
{
    public class TrashCounter : BaseCounter
    {
        public static event EventHandler OnAnyObjectTrashed;
        public override void Interact(Player player)
        {
            if (!player.HasKitchenObject()) return;
            player.GetKitchenObject().DestroySelf();
            OnAnyObjectTrashed?.Invoke(this,EventArgs.Empty);
        }
        
        public new static void ResetStaticData()
        {
            OnAnyObjectTrashed = null;
        }
    }
}

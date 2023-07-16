using System;
using Controller;

namespace Counters
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
    }
}

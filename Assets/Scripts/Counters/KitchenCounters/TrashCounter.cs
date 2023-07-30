using System;
using Controller;
using Unity.Netcode;

namespace Counters.KitchenCounters
{
    public class TrashCounter : BaseCounter
    {
        public static event EventHandler OnAnyObjectTrashed;
        public override void Interact(Player player)
        {
            if (!player.HasKitchenObject()) return;
            KitchenObject.KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            
            InteractLogicServerRpc();
        }
        
        public new static void ResetStaticData()
        {
            OnAnyObjectTrashed = null;
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc()
        {
            InteractLogicClientRpc();
        }
        
        [ClientRpc]
        private void InteractLogicClientRpc()
        {
            OnAnyObjectTrashed?.Invoke(this,EventArgs.Empty);
        }
    }
}

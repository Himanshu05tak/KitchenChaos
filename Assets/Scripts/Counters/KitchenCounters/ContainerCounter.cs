using System;
using Controller;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Counters.KitchenCounters
{
    public class ContainerCounter : BaseCounter
    { 
        [SerializeField] private KitchenObjectSO kitchenObjectSo;
        public event EventHandler OnPlayerGrabObject;
        public override void Interact(Player player)
        {
            if (player.HasKitchenObject()) return;
            //Player isn't carrying anything
            KitchenObject.KitchenObject.SpawnKitchenObject(kitchenObjectSo, player);

            InteractLogicServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc()
        {
            InteractLogicClientRpc();
        }

        [ClientRpc]
        private void InteractLogicClientRpc()
        {
            OnPlayerGrabObject?.Invoke(this,EventArgs.Empty);
        }
    }
}

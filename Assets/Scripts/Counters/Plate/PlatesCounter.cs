using System;
using Manager;
using Controller;
using UnityEngine;
using ScriptableObjects;
using Counters.KitchenCounters;
using Unity.Netcode;

namespace Counters.Plate
{
    public class PlatesCounter : BaseCounter
    {
        public event EventHandler OnPlateSpawned;
        public event EventHandler OnPlateRemoved;
        
        [SerializeField] private KitchenObjectSO plateKitchenObjectSo;
        [SerializeField] private int spawnPlateTimerMax;
        [SerializeField] private int plateSpawnedAmountMax; 

        private float _spawnPlateTimer;
        private int _plateSpawnedAmount;

        public override void Interact(Player player)
        {
            if (player.HasKitchenObject()) return;
            //Player is empty handed
            if (_plateSpawnedAmount <= 0) return;
            //There's at least one plate here
          
            KitchenObject.KitchenObject.SpawnKitchenObject(plateKitchenObjectSo, player);
            InteractLogicServerRpc();
        }
        private void Update()
        {
            if(!IsServer) return;
            _spawnPlateTimer += Time.deltaTime;
            if (!( GameManager.Instance.IsGamePlaying() && _spawnPlateTimer > spawnPlateTimerMax )) return;
                _spawnPlateTimer = 0;
                if ( _plateSpawnedAmount >= plateSpawnedAmountMax) return;
                SpawnPlateServerRpc();
        }

        [ServerRpc]
        private void SpawnPlateServerRpc()
        {
            SpawnPlateClientRpc();
        }

        [ClientRpc]
        private void SpawnPlateClientRpc()
        {
            _plateSpawnedAmount++;
            OnPlateSpawned?.Invoke(this,EventArgs.Empty);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc()
        {
            InteractLogicClientRpc();
        }

        [ClientRpc]
        private void InteractLogicClientRpc()
        {
            _plateSpawnedAmount--;
            OnPlateRemoved?.Invoke(this,EventArgs.Empty);
        }
    }
}

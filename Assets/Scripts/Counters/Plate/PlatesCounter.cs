using System;
using Controller;
using Counters.KitchenCounters;
using Manager;
using ScriptableObjects;
using UnityEngine;

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
            _plateSpawnedAmount--;
            KitchenObject.KitchenObject.SpawnKitchenObject(plateKitchenObjectSo, player);
            OnPlateRemoved?.Invoke(this,EventArgs.Empty);
        }

        private void Update()
        {
            _spawnPlateTimer += Time.deltaTime;
            if (!(GameManager.Instance.IsGamePlaying() && _spawnPlateTimer > spawnPlateTimerMax )) return;
            _spawnPlateTimer = 0;
            if ( _plateSpawnedAmount >= plateSpawnedAmountMax) return;
            _plateSpawnedAmount++;
            OnPlateSpawned?.Invoke(this,EventArgs.Empty);
        }
    }
}

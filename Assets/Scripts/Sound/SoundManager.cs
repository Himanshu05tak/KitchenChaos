using System;
using Controller;
using UnityEngine;
using Counters.Deliver;
using ScriptableObjects;
using Counters.KitchenCounters;
using Random = UnityEngine.Random;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        [SerializeField] private AudioClipRefsSO audioClipRefsSo;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            DeliveryManager.Instance.OnRecipeSuccess += DeliveryManagerOnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
            CuttingCounter.OnAnyCut += CuttingCounterOnOnAnyCut;
            Player.Instance.OnPickSomething += PlayerOnPickSomething;
            BaseCounter.OnAnyObjectPlacedHere += BaseCounterOnAnyObjectPlacedHere;
            TrashCounter.OnAnyObjectTrashed += TrashCounterOnOnAnyObjectTrashed;
        }

        private void TrashCounterOnOnAnyObjectTrashed(object sender, EventArgs e)
        {
            var trashCounter = sender as TrashCounter;
            if (trashCounter != null) PlaySound(audioClipRefsSo.trash, trashCounter.transform.position);
        }

        private void BaseCounterOnAnyObjectPlacedHere(object sender, EventArgs e)
        {
            var baseCounter = sender as BaseCounter;
            if (baseCounter != null) PlaySound(audioClipRefsSo.objectDrop, baseCounter.transform.position);
        }

        private void PlayerOnPickSomething(object sender, EventArgs e)
        {
            PlaySound(audioClipRefsSo.objectPickup, Player.Instance.transform.position);
        }

        private void CuttingCounterOnOnAnyCut(object sender, EventArgs e)
        {
            var cuttingCounter = sender as CuttingCounter;
            if (cuttingCounter != null) PlaySound(audioClipRefsSo.chop, cuttingCounter.transform.position);
        }

        private void DeliveryManagerOnRecipeFailed(object sender, EventArgs e)
        {
            var deliveryCounter = DeliveryCounter.Instance;
            PlaySound(audioClipRefsSo.deliveryFail, deliveryCounter.transform.position);
        }

        private void DeliveryManagerOnRecipeSuccess(object sender, EventArgs e)
        {
            var deliveryCounter = DeliveryCounter.Instance;
            PlaySound(audioClipRefsSo.deliverySuccess, deliveryCounter.transform.position);
        }
    
        private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
        {
            PlaySound(audioClipArray[Random.Range(0,audioClipArray.Length)], position, volume);
        }

        private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
        {
            AudioSource.PlayClipAtPoint(audioClip, position, volume);
        }

        public void PlayFootstepsSound(Vector3 position, float volume)
        {
            PlaySound(audioClipRefsSo.footStep, position, volume);
        }
    }
}

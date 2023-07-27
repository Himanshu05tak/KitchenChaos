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
        private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
        public static SoundManager Instance { get; private set; }
        [SerializeField] private AudioClipRefsSO audioClipRefsSo;

        private float _volume = 1f;
        private void Awake()
        {
            Instance = this;

            _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, _volume);
        }

        void Start()
        {
            DeliveryManager.Instance.OnRecipeSuccess += DeliveryManagerOnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
            CuttingCounter.OnAnyCut += CuttingCounterOnOnAnyCut;
            Player.OnAnyPickedSomething += PlayerOnPickSomething;
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
            var player = sender as Player;
            if (player != null) PlaySound(audioClipRefsSo.objectPickup, player.transform.position);
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
    
        private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplier = 1f)
        {
            PlaySound(audioClipArray[Random.Range(0,audioClipArray.Length)], position, volumeMultiplier);
        }

        private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
        {
            AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * _volume);
        }

        public void PlayFootstepsSound(Vector3 position, float volume)
        {
            PlaySound(audioClipRefsSo.footStep, position, volume);
        }
        public void PlaySoundOnCountDown()
        {
            PlaySound(audioClipRefsSo.warning, Vector3.zero);
        }
        public void PlayWarningSound(Vector3 pos)
        {
            PlaySound(audioClipRefsSo.warning, pos);
        }


        public void ChangeVolume()
        {
            _volume += .1f;
            if (_volume > 1f)
                _volume = 0;

            PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, _volume);
            PlayerPrefs.Save();
        }

        public float GetVolume()
        {
            return _volume;
        }
    }
}

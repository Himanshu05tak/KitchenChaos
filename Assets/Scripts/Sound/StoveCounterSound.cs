using UnityEngine;
using Counters.KitchenCounters;

namespace Sound
{
    public class StoveCounterSound : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            stoveCounter.OnStateChanged += StoveCounterOnOnStateChanged;
        }

        private void StoveCounterOnOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            var playSound = e.FryingState is StoveCounter.FryingState.Frying or StoveCounter.FryingState.Fried;

            if (playSound)
                _audioSource.Play();
            else
                _audioSource.Pause();
        }
    }
}

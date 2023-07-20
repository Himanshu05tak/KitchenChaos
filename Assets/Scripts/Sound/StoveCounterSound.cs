using UnityEngine;
using Counters.KitchenCounters;
using Interface;

namespace Sound
{
    public class StoveCounterSound : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;
        private AudioSource _audioSource;
        private bool _playWarningSound;
        private float _warningSoundTimer;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            stoveCounter.OnStateChanged += StoveCounterOnStateChanged;
            stoveCounter.OnProgressChanged += StoveCounterOnProgressChanged;
        }

        private void StoveCounterOnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            const float burnShowProgressAmount = 0.5f;
            _playWarningSound = stoveCounter.IsFried() && e.ProgressNormalized >= burnShowProgressAmount;
            
        }

        private void StoveCounterOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            var playSound = e.FryingState is StoveCounter.FryingState.Frying or StoveCounter.FryingState.Fried;

            if (playSound)
                _audioSource.Play();
            else
                _audioSource.Pause();
        }

        private void Update()
        {
            if (!_playWarningSound) return;
            _warningSoundTimer -= Time.deltaTime;
            if (!(_warningSoundTimer <= 0)) return;
            const float warningSoundTimerMax = .2f;
            _warningSoundTimer = warningSoundTimerMax;
                
            SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
        }
    }
}

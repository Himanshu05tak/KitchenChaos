using Counters.KitchenCounters;
using Interface;
using UnityEngine;

namespace UI
{
    public class StoveBurnFlashingBarUI : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;
        
        private static readonly int Flashing = UnityEngine.Animator.StringToHash(IS_FLASHING); 
        private const string IS_FLASHING ="IsFlashing";
        private UnityEngine.Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<UnityEngine.Animator>();
        }

        private void Start()
        {
            stoveCounter.OnProgressChanged += StoveCounterOnProgressChanged;
            _animator.SetBool(Flashing, false);

        }

        private void StoveCounterOnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            const float burnShowProgressAmount = 0.5f;
            var showFlashingAnimation = stoveCounter.IsFried() && e.ProgressNormalized >= burnShowProgressAmount;

            _animator.SetBool(Flashing, showFlashingAnimation);
        }
    }
}

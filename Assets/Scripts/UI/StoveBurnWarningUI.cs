using Counters.KitchenCounters;
using Interface;
using UnityEngine;

namespace UI
{
    public class StoveBurnWarningUI : MonoBehaviour
    {
        [SerializeField] private StoveCounter stoveCounter;
        [SerializeField] private Transform warningImg;

        private void Start()
        {
            stoveCounter.OnProgressChanged += StoveCounterOnProgressChanged;
            Hide();
        }

        private void StoveCounterOnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            const float burnShowProgressAmount = 0.5f;
            var show = stoveCounter.IsFried() && e.ProgressNormalized >= burnShowProgressAmount;

            if (show)
            {
                Show();
            }
            else
                Hide();
        }

        private void Show()
        {
            warningImg.gameObject.SetActive(true);
        }
        private void Hide()
        {
            warningImg.gameObject.SetActive(false);
        }
    }
}

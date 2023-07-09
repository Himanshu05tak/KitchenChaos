using UnityEngine;

namespace Counters
{
    public class StoveCounterVisual : MonoBehaviour
    {
        [SerializeField] private GameObject stoveGameObject;
        [SerializeField] private GameObject particleGameObject;

        [SerializeField] private StoveCounter _stoveCounter;
        
        private void Start()
        {
            _stoveCounter.OnStateChanged += StoveCounterOnOnStateChanged;
        }
        private void StoveCounterOnOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            var showVisual = e.FryingState is StoveCounter.FryingState.Frying or StoveCounter.FryingState.Fried;
            stoveGameObject.gameObject.SetActive(showVisual);
            particleGameObject.gameObject.SetActive(showVisual);
        }
    }
}

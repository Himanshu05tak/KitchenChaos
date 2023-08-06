using Counters.KitchenCounters;
using UnityEngine;

namespace Counters.CounterVisuals
{
    public class StoveCounterVisual : MonoBehaviour
    {
        [SerializeField] private GameObject stoveGameObject;
        [SerializeField] private GameObject particleGameObject;

        [SerializeField] private StoveCounter stoveCounter;
        
        private void Start()
        {
            stoveCounter.OnStateChanged += StoveCounterOnOnStateChanged;
        }
        private void StoveCounterOnOnStateChanged(object sender, StoveCounter.StateChangedEventArgs e)
        {
            var showVisual = e.FryingState is StoveCounter.FryingState.Frying or StoveCounter.FryingState.Fried;
            stoveGameObject.gameObject.SetActive(showVisual);
            particleGameObject.gameObject.SetActive(showVisual);
        }
    }
}

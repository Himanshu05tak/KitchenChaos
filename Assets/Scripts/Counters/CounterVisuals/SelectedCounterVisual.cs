using System;
using Controller;
using Counters.KitchenCounters;
using UnityEngine;

namespace Counters.CounterVisuals
{
    public class SelectedCounterVisual : MonoBehaviour
    {
        [SerializeField] private BaseCounter baseCounter;
        [SerializeField] private GameObject[] visualGameObjectArray;
        private void Start()
        {
            if(Player.LocalInstance!=null)
                Player.LocalInstance.OnSelectedCounterCharged += InstanceOnOnSelectedCounterCharged;
            Player.OnAnyPlayerSpawned += OnAnyPlayerSpawned;
        }

        private void OnAnyPlayerSpawned(object sender, EventArgs e)
        {
            if (Player.LocalInstance == null) return;
            Player.LocalInstance.OnSelectedCounterCharged -= InstanceOnOnSelectedCounterCharged;
            Player.LocalInstance.OnSelectedCounterCharged += InstanceOnOnSelectedCounterCharged;
        }

        private void InstanceOnOnSelectedCounterCharged(object sender, Player.OnSelectedCounterChangedEventArgs e)
        {
            if (e.SelectedCounter == baseCounter)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            foreach (var visual in visualGameObjectArray)
            {
                visual.SetActive(true);
            }
      
        }

        private void Hide()
        {
            foreach (var visual in visualGameObjectArray)
            {
                visual.SetActive(false);
            }
        }
    }
}

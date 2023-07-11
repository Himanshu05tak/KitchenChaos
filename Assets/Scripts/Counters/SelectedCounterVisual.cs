using Controller;
using Counters;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private void Start()
    {
        Player.Instance.OnSelectedCounterCharged += InstanceOnOnSelectedCounterCharged;
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

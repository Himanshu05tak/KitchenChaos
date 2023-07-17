using Counters.KitchenCounters;
using UnityEngine;

public class ResetDataStaticManager : MonoBehaviour
{
    private void Awake()
    {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
    }
}

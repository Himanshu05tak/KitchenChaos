using System;
using Counters.KitchenCounters;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    [SerializeField] private ContainerCounter _containerCounter;
    
    private static readonly int OpenClose = UnityEngine.Animator.StringToHash(OPEN_CLOSE);
    private UnityEngine.Animator _animator;
    private const string OPEN_CLOSE ="OpenClose";
    private void Awake()
    {
        _animator = GetComponent<UnityEngine.Animator>();
    }

    private void Start()
    {
        _containerCounter.OnPlayerGrabObject+=ContainerCounterOnOnPlayerGrabObject;
    }

    private void ContainerCounterOnOnPlayerGrabObject(object sender, EventArgs e)
    {
       _animator.SetTrigger(OpenClose);
    }
}

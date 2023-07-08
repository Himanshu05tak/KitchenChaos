using System;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;
    
    private static readonly int OpenClose = UnityEngine.Animator.StringToHash(CUT);
    private UnityEngine.Animator _animator;
    private const string CUT ="Cut";
    private void Awake()
    {
        _animator = GetComponent<UnityEngine.Animator>();
    }

    private void Start()
    {
        cuttingCounter.OnCut +=CuttingCounter_OnCut;
    }

    private void CuttingCounter_OnCut(object sender, EventArgs e)
    {
       _animator.SetTrigger(OpenClose);
    }
}

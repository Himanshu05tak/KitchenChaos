using Controller;
using Unity.Netcode;
using UnityEngine;

namespace Animator
{
    public class PlayerAnimator : NetworkBehaviour
    {
        [SerializeField] private Player playerController;
       
        private static readonly int Walking = UnityEngine.Animator.StringToHash(IsWalking);
        private UnityEngine.Animator _animator;
        private const string IsWalking ="IsWalking";

        private void Awake()
        {
            _animator = GetComponent<UnityEngine.Animator>();
        }
        private void Update()
        {
            if(!IsOwner) return;
            _animator.SetBool(Walking,playerController.IsWalking());
        }
    }
}

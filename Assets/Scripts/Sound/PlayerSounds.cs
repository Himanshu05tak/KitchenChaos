using Controller;
using UnityEngine;

namespace Sound
{
  public class PlayerSounds : MonoBehaviour
  { 
    private Player _player;
    private float _footstepTimer;
    private const float FootstepTimerMax = .1f;
    private void Awake()
    {
      _player = GetComponent<Player>();
    }
    private void Update()
    {
      _footstepTimer -= Time.deltaTime;
      if (!(_footstepTimer < 0f)) return;
      _footstepTimer = FootstepTimerMax;

      if (!_player.IsWalking()) return;
      const float volume = 1;
      SoundManager.Instance.PlayFootstepsSound(_player.transform.position,volume);
    }
  }
}

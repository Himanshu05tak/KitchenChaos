using System;
using Manager;
using UnityEngine;

namespace UI
{
    public class WaitingForOtherPlayersUI : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.OnLocalPlayerReadyChanged += OnLocalPlayerReadyChanged;
            GameManager.Instance.OnStateChanged += OnStateChanged;
            
            Hide();
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if(GameManager.Instance.IsCountdownToStartActive())
                Hide();
        }

        private void OnLocalPlayerReadyChanged(object sender, EventArgs e)
        {
            if(GameManager.Instance.IsLocalPlayerReady())
                Show();
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

using System;
using Manager;
using UnityEngine;

namespace UI
{
    public class PauseMultiplayerUI : MonoBehaviour
    {
        void Start()
        {
            GameManager.Instance.OnMultiplayerGamePaused += OnMultiplayerGamePaused;
            GameManager.Instance.OnMultiplayerGameUnpaused += OnMultiplayerGameUnpaused;
            Hide();
        }

        private void OnMultiplayerGameUnpaused(object sender, EventArgs e)
        {
            Hide();
        }

        private void OnMultiplayerGamePaused(object sender, EventArgs e)
        {
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

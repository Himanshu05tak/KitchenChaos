using System;
using Counters.Deliver;
using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
       [SerializeField] private TextMeshProUGUI totalDelivered;
       private void Start()
        {
            GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
            
            Hide();
        }
        private void GameManagerOnStateChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsGameOver())
            {
                Show();
                totalDelivered.text = DeliveryManager.Instance.GetSuccessfulRecipeDelivered().ToString();
            }
            else
                Hide();
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

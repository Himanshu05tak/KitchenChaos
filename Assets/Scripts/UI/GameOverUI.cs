using TMPro;
using System;
using Manager;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Counters.Deliver;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
       [SerializeField] private TextMeshProUGUI totalDelivered;
       [SerializeField] private Button playAgain;

       private void Awake()
       {
           playAgain.onClick.AddListener(() =>
           {
               NetworkManager.Singleton.Shutdown();
               Loader.Loader.Load(Loader.Loader.Scene.MainMenu);
           });
       }

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

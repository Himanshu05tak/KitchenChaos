using TMPro;
using System;
using Manager;
using UnityEngine;

namespace UI
{
    public class GameCountDownStartUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countDownText;

        private void Start()
        {
            GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
            
            Hide();
        }

        private void Update()
        {
            countDownText.text = Mathf.Ceil(GameManager.Instance.GetCountdownToStartTimer()).ToString();
        }

        private void GameManagerOnStateChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsCountdownToStartActive())
                Show();
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

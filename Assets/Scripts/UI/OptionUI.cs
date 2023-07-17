using System;
using Manager;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionUI : MonoBehaviour
    {
        public static OptionUI Instance { get; private set; }
        [SerializeField] private Button soundEffectBtn;
        [SerializeField] private Button musicBtn;
        [SerializeField] private Button backBtn;
        [SerializeField] private TextMeshProUGUI soundEffectsText;
        [SerializeField] private TextMeshProUGUI musicText;
        
        private void Awake()
        {
            Instance = this;
            soundEffectBtn.onClick.AddListener(() => {SoundManager.Instance.ChangeVolume();
                UpdateVisual();
            });
            musicBtn.onClick.AddListener(() =>
            {
                MusicManager.Instance.ChangeVolume();
                UpdateVisual();
            });
            backBtn.onClick.AddListener(() =>
            {
                Hide();
            });
        }

        private void Start()
        {
            GameManager.Instance.OnGamePause += OnGamePause;
            UpdateVisual();
            Hide();
        }

        private void OnGamePause(object sender, EventArgs e)
        {
            Hide();
        }

        private void UpdateVisual()
        {
            soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
            musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

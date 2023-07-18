using System;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePauseUI : MonoBehaviour
    {
        [SerializeField] private Button resumeBtn;
        [SerializeField] private Button mainMenuBtn;
        [SerializeField] private Button optionBtn;

        private void Awake()
        {
            resumeBtn.onClick.AddListener(()=>GameManager.Instance.ToggledPauseGame());
            mainMenuBtn.onClick.AddListener(()=> Loader.Loader.Load(Loader.Loader.Scene.MainMenu));
            optionBtn.onClick.AddListener(() => { OptionUI.Instance.Show();});
        }

        private void Start()
        {
            GameManager.Instance.OnGamePause += OnGamePause;
            GameManager.Instance.OnGameResume += OnGameResume;
            
            Hide();
        }

        private void OnGamePause(object sender, EventArgs e)
        {
            Show();
        }
        private void OnGameResume(object sender, EventArgs e)
        {
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

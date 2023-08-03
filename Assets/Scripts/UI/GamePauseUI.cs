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
            optionBtn.onClick.AddListener(() => { Hide(); OptionUI.Instance.Show(Show);});
        }

        private void Start()
        {
            GameManager.Instance.OnLocalGamePaused += OnLocalGamePaused;
            GameManager.Instance.OnLocalGameResume += OnLocalGameResume;
            
            Hide();
        }

        private void OnLocalGamePaused(object sender, EventArgs e)
        {
            Show();
        }
        private void OnLocalGameResume(object sender, EventArgs e)
        {
            Hide();
        }

        private void Show()
        {
            gameObject.SetActive(true);
            resumeBtn.Select();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

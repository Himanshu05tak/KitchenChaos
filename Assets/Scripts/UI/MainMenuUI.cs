using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playBtn;
        [SerializeField] private Button quitBtn;

        private void Awake()
        {
            playBtn.onClick.AddListener(() => { Loader.Loader.Load(Loader.Loader.Scene.GamePlay); });
            quitBtn.onClick.AddListener(Application.Quit);
            Time.timeScale = 1;
        }
    }
}

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
            playBtn.onClick.AddListener(() => { Loader.Load(Loader.Scene.GamePlay); });
            quitBtn.onClick.AddListener(Application.Quit);
            Time.timeScale = 1;
        }
    }
}

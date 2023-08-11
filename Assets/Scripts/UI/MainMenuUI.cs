using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button singlePlayerBtn;
        [SerializeField] private Button multiPlayerBtn;
        [SerializeField] private Button quitBtn;

        private void Awake()
        {
            singlePlayerBtn.onClick.AddListener(() =>
            {
                KitchenGameMultiplayer.PlayMultiplayer = false;
                Loader.Loader.Load(Loader.Loader.Scene.LobbyScene);
            });
            multiPlayerBtn.onClick.AddListener(() =>
            {
                KitchenGameMultiplayer.PlayMultiplayer = true;
                Loader.Loader.Load(Loader.Loader.Scene.LobbyScene);
            });
            quitBtn.onClick.AddListener(Application.Quit);
            Time.timeScale = 1;
        }
    }
}

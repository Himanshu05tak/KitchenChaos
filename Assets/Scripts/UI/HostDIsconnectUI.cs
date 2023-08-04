using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HostDisconnectUI : MonoBehaviour
    {
        [SerializeField] private Button playAgainBtn;

        private void Start()
        {
            Hide();
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            playAgainBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.Shutdown();
                Loader.Loader.Load(Loader.Loader.Scene.MainMenu);
            });
        }

        private void OnClientDisconnectCallback(ulong clientID)
        {
            if (clientID == NetworkManager.ServerClientId)
            {
                Show();
            }
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
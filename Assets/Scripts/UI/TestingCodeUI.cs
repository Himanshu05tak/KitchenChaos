using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TestingCodeUI : MonoBehaviour
    {
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button clientBtn;


        private void Awake()
        {
            hostBtn.onClick.AddListener(() => 
            { 
                Debug.Log("HOST"); 
                NetworkManager.Singleton.StartHost();             
                Hide();
            });
            clientBtn.onClick.AddListener(() => 
            {
                Debug.Log("Client");
                NetworkManager.Singleton.StartClient();
                Hide();
            });
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

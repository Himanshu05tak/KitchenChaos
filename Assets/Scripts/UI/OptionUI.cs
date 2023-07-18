using System;
using Input;
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
        
        [Header("BindingKeys")]
        [SerializeField] private TextMeshProUGUI moveUpText;
        [SerializeField] private TextMeshProUGUI moveDownText;
        [SerializeField] private TextMeshProUGUI moveLeftText;
        [SerializeField] private TextMeshProUGUI moveRightText;
        [SerializeField] private TextMeshProUGUI interactText;
        [SerializeField] private TextMeshProUGUI interactAltText;
        [SerializeField] private TextMeshProUGUI pauseText;
        
        [Header("BindingButtons")]
        [SerializeField] private Button moveUpBtn;
        [SerializeField] private Button moveDownBtn;
        [SerializeField] private Button moveLeftBtn;
        [SerializeField] private Button moveRightBtn;
        [SerializeField] private Button interactBtn;
        [SerializeField] private Button interactAltBtn;
        [SerializeField] private Button pauseBtn;
        
        [SerializeField] private Transform pressKeyToRebindTransform;


        
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

            moveUpBtn.onClick.AddListener(() => 
            {
                RebindBinding(PlayerInputController.Bindings.MoveUp);
            });
             moveDownBtn.onClick.AddListener(() => 
            {
                RebindBinding(PlayerInputController.Bindings.MoveDown);
            });
             moveLeftBtn.onClick.AddListener(() => 
             {
                 RebindBinding(PlayerInputController.Bindings.MoveLeft);
             });
             moveRightBtn.onClick.AddListener(() => 
             {
                 RebindBinding(PlayerInputController.Bindings.MoveRight);
             });
             interactBtn.onClick.AddListener(() => 
             {
                 RebindBinding(PlayerInputController.Bindings.Interact);
             });
             interactAltBtn.onClick.AddListener(() => 
             {
                 RebindBinding(PlayerInputController.Bindings.InteractAlt);
             });
             pauseBtn.onClick.AddListener(() => 
             {
                 RebindBinding(PlayerInputController.Bindings.Pause);
             });
        }

        private void Start()
        {
            GameManager.Instance.OnGamePause += OnGamePause;
            UpdateVisual();
            HidePressToRebindKey();
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

            moveUpText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.MoveUp);
            moveDownText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.MoveDown);
            moveLeftText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.MoveLeft);
            moveRightText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.MoveRight);
            interactText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.Interact);
            interactAltText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.InteractAlt);
            pauseText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.Pause);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void ShowPressToRebindKey()
        {
            pressKeyToRebindTransform.gameObject.SetActive(true);
        }

        private void HidePressToRebindKey()
        {
            pressKeyToRebindTransform.gameObject.SetActive(false);
        }

        private void RebindBinding(PlayerInputController.Bindings bindings)
        {
            ShowPressToRebindKey();
            PlayerInputController.Instance.RebindBinding(bindings, () =>
            {
                HidePressToRebindKey(); //If there is a function, It can be replace by MethodGroup
                UpdateVisual();
            });
        }
    }
}

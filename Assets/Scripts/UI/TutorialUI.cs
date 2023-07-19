using System;
using Input;
using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TutorialUI : MonoBehaviour
    {
        [Header("BindingKeys")]
        [SerializeField] private TextMeshProUGUI moveUpText;
        [SerializeField] private TextMeshProUGUI moveDownText;
        [SerializeField] private TextMeshProUGUI moveLeftText;
        [SerializeField] private TextMeshProUGUI moveRightText;
        [SerializeField] private TextMeshProUGUI interactText;
        [SerializeField] private TextMeshProUGUI interactAltText;
        [SerializeField] private TextMeshProUGUI pauseText;
        [Header("BindingForGamePad")]
        [SerializeField] private TextMeshProUGUI gamePadInteractText;
        [SerializeField] private TextMeshProUGUI gamePadInteractAltText;
        [SerializeField] private TextMeshProUGUI gamePadPauseText;

        private void Start()
        {
            PlayerInputController.Instance.OnBindingRebind += OnBindingRebind;
            GameManager.Instance.OnStateChanged += OnStateChanged;
            UpdateVisual();
            Show();
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if(GameManager.Instance.IsCountdownToStartActive())
                Hide();
        }

        private void OnBindingRebind(object sender, EventArgs e)
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            moveUpText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.MoveUp);
            moveDownText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.MoveDown);
            moveLeftText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.MoveLeft);
            moveRightText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.MoveRight);
            interactText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.Interact);
            interactAltText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.InteractAlt);
            pauseText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.Pause);
            gamePadInteractText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.GamePadInteraction);
            gamePadInteractAltText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.GamePadInteractionAlternate);
            gamePadPauseText.text = PlayerInputController.Instance.GetBindingText(PlayerInputController.Bindings.GamePadPause);
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

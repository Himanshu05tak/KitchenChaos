using System;
using Counters.Deliver;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeliveryResultUI : MonoBehaviour
    {
        [SerializeField] private Image backGroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Color successColor;
        [SerializeField] private Color failColor;
        [SerializeField] private Sprite successSprite;
        [SerializeField] private Sprite failSprite;

        private static readonly int Popup = UnityEngine.Animator.StringToHash(DELIVERY_POPUP);
        private UnityEngine.Animator _animator;
        private const string DELIVERY_POPUP ="Popup";
        private void Awake()
        {
            _animator = GetComponent<UnityEngine.Animator>();
        }

        private void Start()
        {
            DeliveryManager.Instance.OnRecipeSuccess += OnDeliveryRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailed += OnDeliveryRecipeFailed;
            Hide();
        }

        private void OnDeliveryRecipeFailed(object sender, EventArgs e)
        {
            Show();
            _animator.SetTrigger(Popup);
            backGroundImage.color = failColor;
            iconImage.sprite = failSprite;
            messageText.text = "DELIVERY\nFAILED";
        }

        private void OnDeliveryRecipeSuccess(object sender, EventArgs e)
        {
            Show();
            _animator.SetTrigger(Popup);
            backGroundImage.color = successColor;
            iconImage.sprite = successSprite;
            messageText.text = "DELIVERED\nSUCCESS";
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

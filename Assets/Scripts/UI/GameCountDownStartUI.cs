using TMPro;
using System;
using Manager;
using Sound;
using UnityEngine;

namespace UI
{
    public class GameCountDownStartUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countDownText;

        private UnityEngine.Animator _animator;
        private int _previousCountDownNumber;
        
        private const string NUMBER_POP = "NumberPopup";
        private static readonly int NumberPopup = UnityEngine.Animator.StringToHash(NUMBER_POP);

        private void Awake()
        {
            _animator = GetComponent<UnityEngine.Animator>();
        }

        private void Start()
        {
            GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
            
            Hide();
        }

        private void Update()
        {
            var currentCountDownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
            countDownText.text = currentCountDownNumber.ToString();

            if (_previousCountDownNumber == currentCountDownNumber) return;
            _previousCountDownNumber = currentCountDownNumber;
            _animator.SetTrigger(NumberPopup);
            SoundManager.Instance.PlaySoundOnCountDown();
        }

        private void GameManagerOnStateChanged(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsCountdownToStartActive())
                Show();
            else
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

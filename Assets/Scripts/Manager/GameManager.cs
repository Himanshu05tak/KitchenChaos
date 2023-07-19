using System;
using Input;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public event EventHandler OnStateChanged;
        public event EventHandler OnGamePause;
        public event EventHandler OnGameResume;
        private GameState _state;
        private float _countDownToStartTimer = 3f;
        private float _gamePlayingTimer;
        private float _gamePlayingTimerMax = 10f;
        private bool _isGamePaused = false;
        private enum GameState
        {
            WaitingToStart,
            CountDownToStart,
            GamePlaying,
            Pause,
            GameOver
        }

        private void Awake()
        {
            Instance = this;
            _state = GameState.WaitingToStart;
        }

        private void Start()
        {
            PlayerInputController.Instance.OnPauseInteraction += GameInputOnPauseAction;
            PlayerInputController.Instance.OnInteractAction += GameInputOnInteractAction;
        }

        private void GameInputOnInteractAction(object sender, EventArgs e)
        {
            if (_state ==  GameState.WaitingToStart)
                _state = GameState.CountDownToStart;
            OnStateChanged?.Invoke(this,EventArgs.Empty);
        }

        private void GameInputOnPauseAction(object sender, EventArgs e)
        {
            ToggledPauseGame();
        }

        private void Update()
        {
            StateMachine();
        }

        private void StateMachine()
        {
            switch (_state)
            {
                case GameState.WaitingToStart:
                    break;
                case GameState.CountDownToStart:
                    _countDownToStartTimer -= Time.deltaTime;
                    if (_countDownToStartTimer < 0f)
                        _state = GameState.GamePlaying;
                    _gamePlayingTimer = _gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this,EventArgs.Empty);

                    break;
                case GameState.GamePlaying:
                    _gamePlayingTimer -= Time.deltaTime;
                    if (_gamePlayingTimer < 0f)
                        _state = GameState.GameOver;
                    OnStateChanged?.Invoke(this,EventArgs.Empty);
                    break;
                case GameState.Pause:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public bool IsGamePlaying()
        {
            return _state == GameState.GamePlaying;
        }

        public bool IsCountdownToStartActive()
        {
            return _state == GameState.CountDownToStart;
        }

        public float GetCountdownToStartTimer()
        {
            return _countDownToStartTimer;
        }

        public bool IsGameOver()
        {
            return _state == GameState.GameOver;
        }

        public float GetPlayingTimerNormalized()
        {
            return 1 - _gamePlayingTimer / _gamePlayingTimerMax;
        }

        public void ToggledPauseGame()
        {
            _isGamePaused = !_isGamePaused;
            if (_isGamePaused)
            {
                Time.timeScale = 0;
                OnGamePause?.Invoke(this,EventArgs.Empty);
            }
            else
            {
                Time.timeScale = 1;
                OnGameResume?.Invoke(this,EventArgs.Empty);
            }
        }
    }
}

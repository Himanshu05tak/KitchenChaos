using System;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public event EventHandler OnStateChanged;
        private GameState _state;
        private float _waitingToStartTimer = 1f;
        private float _countDownToStartTimer = 3f;
        private float _gamePlayingTimer;
        private float _gamePlayingTimerMax = 10f;
        private enum GameState
        {
            WaitingToStart,
            CountDownToStart,
            GamePlaying,
            GameOver
        }

        private void Awake()
        {
            Instance = this;
            _state = GameState.WaitingToStart;
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
                    _waitingToStartTimer -= Time.deltaTime;
                    if (_waitingToStartTimer < 0f)
                        _state = GameState.CountDownToStart;
                    OnStateChanged?.Invoke(this,EventArgs.Empty);
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
                case GameState.GameOver:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Debug.Log(_state);
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
    }
}

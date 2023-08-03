using Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Manager
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }
        public event EventHandler OnStateChanged;
        public event EventHandler OnGamePause;
        public event EventHandler OnGameResume;
        public event EventHandler OnLocalPlayerReadyChanged;
        
        
        private readonly NetworkVariable<GameState> _state = new ();
        private readonly NetworkVariable<float> _countDownToStartTimer = new(3);
        private readonly NetworkVariable<float> _gamePlayingTimer= new ();
        private bool _isGamePaused;
        private bool _isLocalPlayerReady;
        private Dictionary<ulong, bool> _playerReadyDictionary;
        private const float GamePlayingTimerMax = 90f;

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
            _playerReadyDictionary = new Dictionary<ulong, bool>();
        }

        private void Start()
        {
            PlayerInputController.Instance.OnPauseInteraction += GameInputOnPauseAction;
            PlayerInputController.Instance.OnInteractAction += GameInputOnInteractAction;
        }

        public override void OnNetworkSpawn()
        {
            _state.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(GameState previousValue, GameState newValue)
        {
            OnStateChanged?.Invoke(this,EventArgs.Empty);
        }

        private void GameInputOnInteractAction(object sender, EventArgs e)
        {
            if (_state.Value != GameState.WaitingToStart) return;
            _isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this,EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
           _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

           var allClientsAreReady = NetworkManager.Singleton.ConnectedClientsIds.All(clientId => _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId]);

           if (allClientsAreReady)
               _state.Value = GameState.CountDownToStart;
        }

        private void GameInputOnPauseAction(object sender, EventArgs e)
        {
            ToggledPauseGame();
        }

        private void Update()
        {
            if (!IsServer) return;
            StateMachine();
        }

        private void StateMachine()
        {
            switch (_state.Value)
            {
                case GameState.WaitingToStart:
                    break;
                case GameState.CountDownToStart:
                    _countDownToStartTimer.Value -= Time.deltaTime;
                    if (_countDownToStartTimer.Value < 0f)
                        _state.Value = GameState.GamePlaying;
                    _gamePlayingTimer.Value = GamePlayingTimerMax;

                    break;
                case GameState.GamePlaying:
                    _gamePlayingTimer.Value -= Time.deltaTime;
                    if (_gamePlayingTimer.Value < 0f)
                        _state.Value = GameState.GameOver;
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
            return _state.Value == GameState.GamePlaying;
        }

        public bool IsCountdownToStartActive()
        {
            return _state.Value == GameState.CountDownToStart;
        }

        public float GetCountdownToStartTimer()
        {
            return _countDownToStartTimer.Value;
        }

        public bool IsGameOver()
        {
            return _state.Value == GameState.GameOver;
        }

        public float GetPlayingTimerNormalized()
        {
            return 1 - _gamePlayingTimer.Value / GamePlayingTimerMax;
        }

        public bool IsLocalPlayerReady()
        {
            return _isLocalPlayerReady;
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

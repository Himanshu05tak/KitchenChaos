using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class PlayerInputController : MonoBehaviour
    {
        private const string PLAYER_PREFS_BINDINGS = "PlayerInputBindings";

        public enum Bindings
        {
            MoveUp,
            MoveDown,
            MoveLeft,
            MoveRight,
            Interact,
            InteractAlt,
            Pause,
            GamePadInteraction,
            GamePadInteractionAlternate,
            GamePadPause
        }
        public static PlayerInputController Instance;
        
        private PlayerInputAction _playerInputAction;
        public event EventHandler OnInteractAction;
        public event EventHandler OnInteractAlternateAction;
        public event EventHandler OnPauseInteraction;
        public event EventHandler OnBindingRebind;

        private void Awake()
        {
            Instance = this;
            _playerInputAction = new PlayerInputAction();
            
            if(PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
                _playerInputAction.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
            
            _playerInputAction.PlayerMove.Enable();
            _playerInputAction.PlayerMove.Interact.performed += InteractPerformed;
            _playerInputAction.PlayerMove.InteractAlternate.performed += InteractAlternatePerformed;
            _playerInputAction.PlayerMove.Pause.performed += PausePerformed;
            
           
        }

        private void OnDestroy()
        {
            _playerInputAction.PlayerMove.Interact.performed -= InteractPerformed;
            _playerInputAction.PlayerMove.InteractAlternate.performed -= InteractAlternatePerformed;
            _playerInputAction.PlayerMove.Pause.performed -= PausePerformed;
            
            _playerInputAction.Dispose();
        }

        private void PausePerformed(InputAction.CallbackContext obj)
        {
            OnPauseInteraction?.Invoke(this,EventArgs.Empty);
        }

        private void InteractAlternatePerformed(InputAction.CallbackContext ctx)
        {
            OnInteractAlternateAction?.Invoke(this,EventArgs.Empty);
        }

        private void InteractPerformed(InputAction.CallbackContext ctx)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }

        public Vector2 GetMovementVectorNormalized()
        {
            return  _playerInputAction.PlayerMove.Move.ReadValue<Vector2>().normalized;
        }

        public string GetBindingText(Bindings bindings)
        {
            return bindings switch
            {
                Bindings.MoveUp => _playerInputAction.PlayerMove.Move.bindings[1].ToDisplayString(),
                Bindings.MoveDown => _playerInputAction.PlayerMove.Move.bindings[2].ToDisplayString(),
                Bindings.MoveLeft => _playerInputAction.PlayerMove.Move.bindings[3].ToDisplayString(),
                Bindings.MoveRight => _playerInputAction.PlayerMove.Move.bindings[4].ToDisplayString(),
                Bindings.Interact => _playerInputAction.PlayerMove.Interact.bindings[0].ToDisplayString(),
                Bindings.InteractAlt => _playerInputAction.PlayerMove.InteractAlternate.bindings[0].ToDisplayString(),
                Bindings.Pause => _playerInputAction.PlayerMove.Pause.bindings[0].ToDisplayString(),
                Bindings.GamePadInteraction => _playerInputAction.PlayerMove.Interact.bindings[1].ToDisplayString(),
                Bindings.GamePadInteractionAlternate => _playerInputAction.PlayerMove.InteractAlternate.bindings[1].ToDisplayString(),
                Bindings.GamePadPause => _playerInputAction.PlayerMove.Pause.bindings[1].ToDisplayString(),
                _ => throw new ArgumentOutOfRangeException(nameof(bindings), bindings, null)
            };
        }

        public void RebindBinding(Bindings bindings, Action onActionRebound)
        {
            _playerInputAction.PlayerMove.Disable();

            InputAction inputAction;
            int bindingIndex;
            switch (bindings)
            {
                case Bindings.MoveUp:
                    inputAction = _playerInputAction.PlayerMove.Move;
                    bindingIndex = 1;
                    break;
                case Bindings.MoveDown:
                    inputAction = _playerInputAction.PlayerMove.Move;
                    bindingIndex = 2;
                    break;
                case Bindings.MoveLeft:
                    inputAction = _playerInputAction.PlayerMove.Move;
                    bindingIndex = 3;
                    break;
                case Bindings.MoveRight:
                    inputAction = _playerInputAction.PlayerMove.Move;
                    bindingIndex = 4;
                    break;
                case Bindings.Interact:
                    inputAction = _playerInputAction.PlayerMove.Interact;
                    bindingIndex = 0;
                    break;
                case Bindings.InteractAlt:
                    inputAction = _playerInputAction.PlayerMove.InteractAlternate;
                    bindingIndex = 0;
                    break;
                case Bindings.Pause:
                    inputAction = _playerInputAction.PlayerMove.Pause;
                    bindingIndex = 0;
                    break;
                case Bindings.GamePadInteraction:
                    inputAction = _playerInputAction.PlayerMove.Interact;
                    bindingIndex = 1;
                    break;
                case Bindings.GamePadInteractionAlternate:
                    inputAction = _playerInputAction.PlayerMove.InteractAlternate;
                    bindingIndex = 1;
                    break;
                case Bindings.GamePadPause:
                    inputAction = _playerInputAction.PlayerMove.Pause;
                    bindingIndex = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bindings), bindings, null);
            }
            inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
            {
                callback.Dispose();
                _playerInputAction.PlayerMove.Enable();
                onActionRebound();
                
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS,_playerInputAction.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
                OnBindingRebind?.Invoke(this,EventArgs.Empty);
            }).Start();
        }
    }
}

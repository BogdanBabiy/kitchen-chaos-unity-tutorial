using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    public static GameInput Instance { get; private set; }
    
    public event EventHandler onInteractAction;
    public event EventHandler onInteractAlternateAction;
    public event EventHandler onPauseAction;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Right,
        Move_Left,
        Interact,
        InteractAlternate,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause
    }
    
    private PlayerInputActions playerInputActions;
    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) 
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));

        playerInputActions.Player.Enable();
        
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;
        
        playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        onPauseAction?.Invoke(this,EventArgs.Empty);
    }

    private void InteractAlternate_performed(InputAction.CallbackContext obj)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        onInteractAlternateAction?.Invoke(this,EventArgs.Empty);
    }

    // Invokes the onInteractAction event
    private void Interact_performed(InputAction.CallbackContext obj)
    {
        if (KitchenGameManager.Instance.IsGameOver() || KitchenGameManager.Instance.IsCountdownToStartActive()) return;
        // Null check for listeners
       onInteractAction?.Invoke(this,EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalised()
    {
        if (KitchenGameManager.Instance.IsGameWaitingToStart()) return Vector2.zero;
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector.normalized;
    }

    public string GetBindingText(Binding binding)
    {
      switch (binding)
        {
            default:
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Gamepad_Interact:
                return playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return playerInputActions.Player.Pause.bindings[1].ToDisplayString();
        }
    }

    public void Rebind(Binding binding, Action onActionRebound)
    {
        playerInputActions.Player.Disable();

        (InputAction inputAction, int bindingIndex) = binding switch
        {
            Binding.Interact => (playerInputActions.Player.Interact, 0),
            Binding.InteractAlternate => (playerInputActions.Player.InteractAlternate, 0),
            Binding.Pause => (playerInputActions.Player.Pause, 0),
            Binding.Move_Up => (playerInputActions.Player.Move, 1),
            Binding.Move_Down => (playerInputActions.Player.Move, 2),
            Binding.Move_Right => (playerInputActions.Player.Move, 3),
            Binding.Move_Left => (playerInputActions.Player.Move, 4),
            Binding.Gamepad_Interact => (playerInputActions.Player.Interact, 1),
            Binding.Gamepad_InteractAlternate => (playerInputActions.Player.InteractAlternate, 1),
            Binding.Gamepad_Pause => (playerInputActions.Player.Pause, 1),
            _ => (playerInputActions.Player.Interact,0)
        };
        
        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();
                
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS,playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }
}

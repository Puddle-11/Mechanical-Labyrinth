using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayInputManager : MonoBehaviour
{
    public static GameplayInputManager instance;
    private PlayerInput _playerInput;

    private InputAction pause;
    private InputAction use;
    private InputAction pickup;
    private InputAction aim;
    private InputAction look;
    private InputAction movement;
    private InputAction reload;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Two Gameplay Input Managers found\nDestroyed at " + gameObject.name);
            Destroy(this);
        }

        InitializeActions();
    }
    private void InitializeActions()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();
        reload = _playerInput.Player.Reload;
        pause = _playerInput.Player.Pause;
        use = _playerInput.Player.Fire;
        pickup = _playerInput.Player.Pickup;
        aim = _playerInput.Player.Aim;
        look = _playerInput.Player.Look;
        movement = _playerInput.Player.Move;
    }

    #region Button Input Methods
    public bool OnReload() {return reload.WasPerformedThisFrame();}
    public bool OnUseDown() { return use.WasPressedThisFrame();}
    public bool OnUseUp() { return use.WasReleasedThisFrame();}
    public bool OnUse() {return use.WasPerformedThisFrame(); }
    public bool OnPause() {return pause.WasPerformedThisFrame();}
    public bool OnAim() { return aim.WasPerformedThisFrame(); }
    public bool OnPickup() { return pickup.WasPerformedThisFrame(); }
    #endregion

    public Vector2 MovementInput()
    {
        return movement.ReadValue<Vector2>();
    }
    public Vector2 LookInput()
    {
        return look.ReadValue<Vector2>();
    }

}

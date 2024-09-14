using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayInputManager : MonoBehaviour
{
    public static GameplayInputManager instance;
    [SerializeField] private PlayerInput _playerInput;

    private InputAction pause;
    private InputAction use;
    private InputAction pickup;
    private InputAction aim;
    private InputAction look;
    private InputAction movement;
    private InputAction reload;
    private InputAction UIClick;
    private InputAction drop;
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
    private void RemapKey()
    {

    }
    private void InitializeActions()
    {
        reload = _playerInput.actions["Reload"];
        pause = _playerInput.actions["Pause"];
        use = _playerInput.actions["Fire"];
        pickup = _playerInput.actions["Pickup"];
        aim = _playerInput.actions["Aim"];
        look = _playerInput.actions["Look"];
        movement = _playerInput.actions["Move"];
        drop = _playerInput.actions["Drop"];
    }

    #region Button Input Methods
    public bool OnDrop() {return drop.WasPerformedThisFrame();}
    public bool OnUIClick() { return UIClick.WasPerformedThisFrame(); }
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

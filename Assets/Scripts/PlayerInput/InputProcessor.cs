using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputProcessor
{
    private PlayerInput playerInput;
    [NonSerialized] public InputAction changeLaneAction;
    [NonSerialized] public InputAction jumpAction;
    [NonSerialized] public InputAction dashAction;
    [NonSerialized] public InputAction mousePosAction;
    [NonSerialized] public InputAction timeScaleAction;

    [NonSerialized] public bool jumpInput;
    [NonSerialized] public float changeLaneAxis;
    public InputBuffer buffer { get; private set; }

    public InputProcessor(PlayerInput _playerInput, MonoBehaviour _coroutineStarter, float _bufferTimeout)
    {
        playerInput = _playerInput;
        SetAllInputActions();
        DelegateInputs();
        buffer = new InputBuffer(_coroutineStarter, _bufferTimeout);
    }

    private void SetAllInputActions()
    {
        changeLaneAction = playerInput.actions["ChangeLane"];
        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Dash"];
        timeScaleAction = playerInput.actions["TimeScale"];
        mousePosAction = playerInput.actions["MousePosition"];
    }

    private void DelegateInputs()
    {
        jumpAction.started += JumpInput;
        jumpAction.canceled += EndJump;
    }

    public void UnsubscribeInputs()
    {
        jumpAction.started -= JumpInput;
        jumpAction.canceled -= EndJump;
    }

    public Vector3 GetMouseWorldPosition(Camera _mainCam, float _targetZ)
    {
        Vector3 mousePosition = mousePosAction.ReadValue<Vector2>();
        mousePosition.z = _targetZ - _mainCam.transform.position.z;

        return _mainCam.ScreenToWorldPoint(mousePosition);
    }

    public Vector3 GetDashDirection(Camera _mainCam, Vector3 _charPosition)
    {
        return (GetMouseWorldPosition(_mainCam, _charPosition.z) - _charPosition).normalized;
    }

    public void JumpInput(InputAction.CallbackContext _context)
    {        
        jumpInput = true;
        buffer.RegisterInput("jump");
    }

    public void EndJump(InputAction.CallbackContext _context)
    {        
        jumpInput = false;
    }
 

}

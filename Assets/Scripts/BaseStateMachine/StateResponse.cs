using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class StateResponse
{
    protected PlayerStateMachineBusiness business;
    protected StateMachine<PlayerState> stateMachine;

    protected void Init(PlayerStateMachineBusiness _business)
    {
        business = _business;
        stateMachine = business.stateMachine;
    }


    public virtual void RecieveInput(InputAction.CallbackContext _context) { return; }
    public virtual void EndInput(InputAction.CallbackContext _context) { return; }

    public virtual void PerformAction() { return; }
}

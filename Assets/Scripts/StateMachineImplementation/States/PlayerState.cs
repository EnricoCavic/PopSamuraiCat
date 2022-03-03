using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerState : State
{
    protected PlayerStateMachineBusiness business;

    public virtual void Init(PlayerStateMachineBusiness _business)
    {
        business = _business;
    }
    
    public virtual void MoveInput() { return; }

    public virtual void ChangeLane(float _input)
    {
        business.changeLaneResponse.PerformAction(_input);
        business.stateMachine.NewState(business.GetState("changingLane"));    
    }

    public virtual void JumpInput() { return; }
    
    public virtual void JumpEnd() { return; }

    public virtual void DashInputStart()
    {
        business.stateMachine.NewState(business.GetState("chargingDash"));
    }

    public virtual void DashInputEnd() { return; }

    public virtual void StateTimeout(PlayerState _currentState) { return; }


}

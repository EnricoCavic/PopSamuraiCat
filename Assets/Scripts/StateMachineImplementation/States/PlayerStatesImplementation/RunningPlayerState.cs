using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;


public class RunningPlayerState : PlayerState
{
    private bool coyoteTime = false;
    private GameEventObject animationEvent;
    public RunningPlayerState(PlayerStateMachineBusiness _business)
    {
        Init(_business);
            
        foreach(GameEventObject _e in business.eventList)
        {
            if(_e.tag == "RunAnimation")
            {
                animationEvent = _e;
            }
        }
    }

    public override void Enter()
    {
        coyoteTime = false;
        if(business.inputProcessor.buffer.CheckNextInputInBuffer("jump"))
            Jump();

        animationEvent.Raise();
    }

    public override State Tick()
    {        
        if(!business.gameplayManager.IsGrounded() && !coyoteTime)
        {
            business.StartCoyoteTime();
            coyoteTime = true;
        }
            

        return this;
    }

    public override State FixedTick()
    {
        if(!business.gameplayManager.CanMoveForward())
            return business.GetState("idle");

        business.MoveAction();
        return this;
    }

    public override void JumpInput() => Jump();


    public void Jump()
    {
        business.stateMachine.NewState(business.GetState("jumping"));
        business.jumpResponse.PerformAction();
    }

    public override void StateTimeout(PlayerState _currentState)
    {
        if(_currentState == this)
            business.stateMachine.NewState(business.GetState("airborne"));
    }
}

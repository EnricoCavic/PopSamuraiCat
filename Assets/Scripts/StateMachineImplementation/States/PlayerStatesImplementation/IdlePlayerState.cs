using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdlePlayerState : PlayerState
{
    public IdlePlayerState(PlayerStateMachineBusiness _business)
    {
        Init(_business);
    }

    public override void Enter()
    {
        if(business.inputProcessor.buffer.CheckNextInputInBuffer("jump"))
            Jump();        
    }

    public override State Tick()
    {
        if(!business.gameplayManager.IsGrounded())
            return business.GetState("airborne");
                 
        return this;
    }

    public override void JumpInput() => Jump();


    public override void MoveInput()
    {   
        if(business.gameplayManager.CanMoveForward())
            business.stateMachine.NewState(business.GetState("running"));
    }

    public void Jump()
    {
        business.stateMachine.NewState(business.GetState("jumping"));
        business.jumpResponse.PerformAction();
    }

}

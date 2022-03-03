using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JumpingPlayerState : PlayerState
{
    private bool jumpCanEnd = false;
    private GameEventObject eventObject;
    private GameEventObject animationEventObject;
    public JumpingPlayerState(PlayerStateMachineBusiness _business)
    {
        Init(_business);

        foreach(GameEventObject _e in business.eventList)
        {
            if(_e.tag == "Jump")
            {
                eventObject = _e;
                
            }  
            if(_e.tag == "JumpAnimation")
            {
                animationEventObject = _e;
                
            }              
        }
    }

    public override void Enter()
    {
        business.gameplayManager.SetDrag(this);
        jumpCanEnd = false;
        eventObject.Raise();
        animationEventObject.Raise();

    }

    public override State Tick()
    {
        if(!jumpCanEnd)
            return this;

        if(business.gameplayManager.IsFalling() || !business.inputProcessor.jumpInput)
            return business.GetState("airborne");

        return this;

    }

    public override State FixedTick()
    {
        if(business.gameplayManager.CanMoveForward())
            business.MoveAction();

        business.gameplayManager.ApplyGravityMultiplier(this);
        
        return this;
    }

    public override void JumpEnd()
    {
        jumpCanEnd = true;
    }


    public override void Exit()
    {
        business.gameplayManager.rb.drag = business.gameplayManager.variables.defaultDrag;
    }

}

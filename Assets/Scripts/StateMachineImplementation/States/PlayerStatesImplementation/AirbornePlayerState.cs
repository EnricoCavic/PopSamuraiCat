using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirbornePlayerState : PlayerState
{
    private GameEventObject animationEventObject;
    public AirbornePlayerState(PlayerStateMachineBusiness _business)
    {
        Init(_business);

        foreach(GameEventObject _e in business.eventList)
        {
            if(_e.tag == "JumpAnimation")
            {
                animationEventObject = _e;
                
            }              
        }        
    }

    public override void Enter()
    {
        business.gameplayManager.SetDrag(this);
        animationEventObject.Raise();
    }

    public override State Tick()
    {
        if(business.gameplayManager.IsGrounded() && business.gameplayManager.IsFalling())
            return business.GetState("running");

        return this;
    }

    public override State FixedTick()
    {
        business.gameplayManager.ApplyGravityMultiplier(business.gameplayManager.variables.airbourneGravityMultiplier);

        if(business.gameplayManager.CanMoveForward())
            business.MoveAction();

        return this;
    }

    public override void Exit()
    {
        business.gameplayManager.SetDragToDefault();
    }


}

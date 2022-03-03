using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingLanePlayerState : PlayerState
{
    public ChangingLanePlayerState(PlayerStateMachineBusiness _business)
    {
        Init(_business);
    }

    public override State Tick()
    {
        if(business.gameplayManager.GetDistanceToLane() <= 0.01f)
        {
            if(!business.gameplayManager.IsGrounded())
                return business.GetState("airborne");     

            return business.GetState("running");   
        }
            
        return this;
        
    }

    public override State FixedTick()
    {
        business.MoveAction();
        return this;
    }

    public override void DashInputStart() { return; }

    public override void ChangeLane(float _input) { return; }


}

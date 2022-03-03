using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashingPlayerState : PlayerState
{
    private GameEventObject eventObject;
    private GameEventObject dashAnimationEvent;
    public DashingPlayerState(PlayerStateMachineBusiness _business)
    {
        Init(_business);

        foreach(GameEventObject _e in business.eventList)
        {
            switch(_e.tag)
            {
                case "Dash":
                    eventObject = _e;
                    break;

                case "DashAnimation":
                    dashAnimationEvent = _e;
                    break;

                default:
                    break;                                        
            }                 
        }
    }
    public override void Enter()
    {
        business.gameplayManager.SetDrag(this);
        business.gameplayManager.rb.angularVelocity = Vector3.zero;;
        eventObject.Raise();
        dashAnimationEvent.Raise();
    }

    public override State Tick()
    {
        return this;
    }

    public override State FixedTick()
    {
        business.gameplayManager.ApplyGravityMultiplier(this);
        business.dashResponse.PerformAction(this);
        return this;
    }

    public override void Exit()
    {
        business.gameplayManager.SetDragToDefault();
        
    }

    public override void DashInputStart() { }

    public override void StateTimeout(PlayerState _currentState)
    {
        if(_currentState == this)
            business.stateMachine.NewState(business.GetState("airborne"));
    }

}

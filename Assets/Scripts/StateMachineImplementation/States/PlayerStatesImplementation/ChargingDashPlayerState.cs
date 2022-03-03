using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingDashPlayerState : PlayerState
{
    private float dashPower; 
    private GameEventObject chargeStartEvent;
    private GameEventObject chargeEndEvent;
    private GameEventObject chargeAnimationEvent;
    public ChargingDashPlayerState(PlayerStateMachineBusiness _business)
    {
        Init(_business);

        foreach(GameEventObject _e in business.eventList)
        {
            switch(_e.tag)
            {
                case "Charge":
                    chargeStartEvent = _e;
                    break;

                case "ChargeAnimation":
                    chargeAnimationEvent = _e;
                    break;

                case "DashFail":
                    chargeEndEvent = _e;
                    break;

                default:
                    break;                                        
            }                 
        }
    }

    public override void Enter()
    {
        business.gameplayManager.SetDrag(this);
        dashPower = 0f;
        chargeStartEvent?.Raise();
        chargeAnimationEvent?.Raise();
    }

    public override State Tick()
    {
        return this;
    }

    public override State FixedTick()
    {
        dashPower += Time.fixedDeltaTime;
        business.gameplayManager.ApplyGravityMultiplier(this);
        business.MoveAction(this);
        return this;
    }

    public override void Exit()
    {
        business.gameplayManager.SetDragToDefault();
        
        if(business.dashResponse.chargeTimeout != null)
            business.StopCoroutine(business.dashResponse.chargeTimeout);
    }


    public override void DashInputStart() { return; }

    public override void DashInputEnd()
    {
        business.stateMachine.NewState(business.GetState("dashing"));
        business.dashResponse.PerformAction(GetPowerPercentage());
    }


    public override void StateTimeout(PlayerState _currentState)
    {
        if(_currentState == this)
            business.stateMachine.NewState(business.GetState("airborne"));

        chargeEndEvent.Raise();
    }

    private float GetPowerPercentage() => Mathf.Pow(dashPower, 1f/5f ) /business.gameplayVariables.chargingDashTimeout;

}



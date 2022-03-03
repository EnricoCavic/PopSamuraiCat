using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerDashResponse : StateResponse
{
    public IEnumerator chargeTimeout { get; private set; }

    public Vector3 direction;

    public bool canStartCharge;
    private GameEventObject eventObject;

    public PlayerDashResponse(PlayerStateMachineBusiness _business)
    {
        base.Init(_business);
        canStartCharge = true;

        foreach(GameEventObject _e in business.eventList)
        {
            if(_e.tag == "Cooldown")
            {
                eventObject = _e;
                break;
            }              
        }
        
    }
    public override void RecieveInput(InputAction.CallbackContext _context)
    {
        if(!canStartCharge)
            return;

        stateMachine?.currentState.DashInputStart();
        chargeTimeout = ChargingStateTimeout();
        business.StartCoroutine(chargeTimeout);        
    }

    public IEnumerator ChargingStateTimeout()
    {
        yield return new WaitForSeconds(business.gameplayVariables.chargingDashTimeout);
        chargeTimeout = null;
        stateMachine?.currentState.StateTimeout(business.GetState("chargingDash"));
    }

    public override void EndInput(InputAction.CallbackContext _context)
    {
        stateMachine?.currentState.DashInputEnd();
    }




    public void PerformAction(float _dashMultiplier) 
    {
        if(!business.gameObject.activeSelf)
            return;
        
        business.StartCoroutine(ChargingDashCooldown());
        canStartCharge = false;
        direction = business.inputProcessor.GetDashDirection(business.mainCam, business.transform.position) * _dashMultiplier * business.gameplayManager.variables.dashChargeForceWeight;
        business.gameplayManager.Dash(direction);
        business.StartCoroutine(DashStateTimeout(_dashMultiplier * business.gameplayManager.variables.dashChargeTimeoutWeight));
    }

    public IEnumerator ChargingDashCooldown()
    {
        yield return new WaitForSeconds(business.gameplayVariables.dashCooldown);
        canStartCharge = true;
        eventObject.Raise();
    }

    public void PerformAction(DashingPlayerState _currentState) 
    {
        business.gameplayManager.Dash(direction);
    }

    public IEnumerator DashStateTimeout(float _dashMultiplier)
    {
        yield return new WaitForSeconds(business.gameplayVariables.dashTimeout * _dashMultiplier);
        stateMachine?.currentState.StateTimeout(business.GetState("dashing"));
    }




}

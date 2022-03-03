using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChangeLaneResponse : StateResponse
{
    GameplayManager gameplayManager;
    public PlayerChangeLaneResponse(PlayerStateMachineBusiness _business)
    {
        base.Init(_business);
        gameplayManager = business.gameplayManager;
    }

    public override void RecieveInput(InputAction.CallbackContext _context)
    {
        if(!gameplayManager.CanChangeLane())
            return;

        gameplayManager.currentLane *= -1f;

        stateMachine?.currentState.ChangeLane(gameplayManager.currentLane);        
    }

    public void PerformAction(float _lane)
    {
        if(!gameplayManager.CanChangeLane())
            return;

        gameplayManager.ChangeLane(_lane);
        business.StartCoroutine(ChangeLaneCoroutine(_lane));        
    }

    public IEnumerator ChangeLaneCoroutine(float _lane)
    {
        yield return new WaitForEndOfFrame();

        if(gameplayManager.GetDistanceToLane() > 0.01f )
        {
            gameplayManager.ChangeLane(_lane);
            business.StartCoroutine(ChangeLaneCoroutine(_lane));
        }
          
    }
}

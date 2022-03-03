using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerJumpResponse : StateResponse
{
    public PlayerJumpResponse(PlayerStateMachineBusiness _business)
    {
        base.Init(_business);
    }

    public override void RecieveInput(InputAction.CallbackContext _context)
    {
        stateMachine?.currentState.JumpInput();
    }

    public override void PerformAction()
    {
        business.StartCoroutine(JumpStateTimeout());
        business.inputProcessor.buffer.RemoveInput();
        business.gameplayManager.Jump();        
    }

    public IEnumerator JumpStateTimeout()
    {
        yield return new WaitForFixedUpdate();
        stateMachine?.currentState.JumpEnd();
    }
    
}

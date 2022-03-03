using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager 
{

    public GameplayVariablesObject variables;
    public Rigidbody rb;
    public Transform myTransform;
    private BoxCastManager castManager;
    private Vector3 GRAVITY;
    public float currentLane;


    public GameplayManager(Transform _transform, Rigidbody _rb, BoxCastManager _bm, GameplayVariablesObject _variables)
    {
        myTransform = _transform;
        rb = _rb;
        castManager = _bm;
        variables = _variables;
        GRAVITY = Physics.gravity;
        currentLane = myTransform.position.z;
    }

    public void ChangeLane(float _lane)
    {
        myTransform.position = Vector3.Lerp(myTransform.position, 
                                            new Vector3(myTransform.position.x , myTransform.position.y, _lane),
                                            0.6f);
    }

    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, variables.jumpForce, rb.velocity.z);        
    }

    public void Move(float _moveAxis)
    {
        rb.AddForce(_moveAxis * variables.movementSpeed, 0, 0, ForceMode.Force);     
    }

    public void Dash(Vector3 _dashDirection)
    {
        rb.velocity = _dashDirection * variables.dashForce;
    }

    public void ApplyGravityMultiplier(JumpingPlayerState _curentState) => ApplyGravityMultiplier(variables.jumpingGravityMultiplier);
    public void ApplyGravityMultiplier(AirbornePlayerState _curentState) => ApplyGravityMultiplier(variables.airbourneGravityMultiplier);
    public void ApplyGravityMultiplier(DashingPlayerState _curentState) => ApplyGravityMultiplier(variables.dashingGravityMultiplier);
    public void ApplyGravityMultiplier(ChargingDashPlayerState _curentState) => ApplyGravityMultiplier(variables.chargingDashGravityMultiplier);
    public void ApplyGravityMultiplier(float _gravityMultiplier)
    {
        Vector3 resultVector = GRAVITY * _gravityMultiplier - GRAVITY;
        rb.AddForce(resultVector, ForceMode.Acceleration);
    }
    
    public bool IsGrounded() => castManager.ChecarBoxCast("GroundCheck");

    public bool CanMoveForward() => !castManager.ChecarBoxCast("RightWallCheck");

    public bool IsFalling() => rb.velocity.y < 0.5f;

    public bool IsMoving() => Mathf.Abs(rb.velocity.x) > 0.1f;

    public bool CanChangeLane() => !castManager.ChecarBoxCast("FrontWallCheck") && !castManager.ChecarBoxCast("BackWallCheck");

    public float GetDistanceToLane() => 1 - Mathf.Abs(myTransform.position.z);

    public void SetDragToDefault() => rb.drag = variables.defaultDrag;
    public void SetDrag(AirbornePlayerState _curentState) => rb.drag = variables.airDrag;
    public void SetDrag(JumpingPlayerState _curentState) => rb.drag = variables.airDrag;
    public void SetDrag(DashingPlayerState _curentState) => rb.drag = variables.dashingDrag;
    public void SetDrag(ChargingDashPlayerState _curentState) => rb.drag = variables.chargingDashDrag;


}
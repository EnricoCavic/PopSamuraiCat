using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Gameplay Variables", menuName = "ScriptableObjects/Gameplay Variables")]
public class GameplayVariablesObject : ScriptableObject
{
    [Header("Movement")]
    public float jumpForce = 15f;
    public float movementSpeed = 40f;
    public float defaultDrag = 6f;
    public float coyoteTime = 0.2f;

    [Header("Dash")]
    public float dashForce = 30f;
    public float dashChargeForceWeight = 1.4f;
    public float dashTimeout = 0.5f;
    public float dashChargeTimeoutWeight = 0.7f;
    public float dashingDrag = 4f;
    public float dashingGravityMultiplier = 0.5f;
    public float dashCooldown = 1f;

    [Header("ChargingDash")]
    public float chargingDashSpeedMultiplier = 0.5f;
    public float chargingDashTimeout = 1f;
    public float chargingDashGravityMultiplier = 0.2f;
    public float chargingDashDrag = 8f;


    [Header("Airborne")]
    public float airDrag = 4f;
    public float airbourneGravityMultiplier = 8f;
    public float jumpingGravityMultiplier = 0.5f;


}

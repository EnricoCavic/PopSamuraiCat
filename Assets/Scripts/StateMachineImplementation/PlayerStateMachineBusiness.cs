using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerStateMachineBusiness : StateMachineBusiness<PlayerState>
{
    public Camera mainCam { get; private set; }
    public InputProcessor inputProcessor;
    public GameplayManager gameplayManager;
    public GameplayVariablesObject gameplayVariables;
    public List<GameEventObject> eventList;
    [SerializeField] private float inputBufferTimeout;

    public float xAxis = 1f;
    public float deathZone = -5f;
    
    public PlayerJumpResponse jumpResponse;
    public PlayerDashResponse dashResponse;
    public PlayerChangeLaneResponse changeLaneResponse;
  
    private void Awake() 
    {
        gameplayManager = new GameplayManager(transform, GetComponent<Rigidbody>(), GetComponent<BoxCastManager>(), gameplayVariables);
        inputProcessor = new InputProcessor(GetComponent<PlayerInput>(), this, inputBufferTimeout);
        mainCam = Camera.main;

        InitializeStates();
        InitializeResponses();

    }

    private void Update() 
    {
        if(transform.position.y < deathZone)
            GetComponent<SceneChanger>().ChangeScene();

        TickStateMachine();    
    }

    private void FixedUpdate() 
    {
        stateMachine?.currentState.MoveInput();
        FixedTickStateMachine();    
    }

    private void OnDestroy() 
    {
        StopAllCoroutines();
        UnsubscribeResponses();  
        inputProcessor.UnsubscribeInputs();  
    }

    public override void InitializeStates()
    {
        base.InitializeStates();

        AddState("idle", new IdlePlayerState(this));
        AddState("jumping", new JumpingPlayerState(this));
        AddState("airborne", new AirbornePlayerState(this));
        AddState("running", new RunningPlayerState(this));
        AddState("changingLane", new ChangingLanePlayerState(this));
        AddState("dashing", new DashingPlayerState(this));
        AddState("chargingDash", new ChargingDashPlayerState(this));
        stateMachine = new StateMachine<PlayerState>(GetState("idle"));
    }

    private void InitializeResponses()
    {
        jumpResponse = new PlayerJumpResponse(this);
        dashResponse = new PlayerDashResponse(this);
        changeLaneResponse = new PlayerChangeLaneResponse(this);

        SubscribeResponses();
    }

    private void SubscribeResponses()
    {
        inputProcessor.jumpAction.started += jumpResponse.RecieveInput;
        inputProcessor.dashAction.started += dashResponse.RecieveInput;
        inputProcessor.dashAction.canceled += dashResponse.EndInput;
        inputProcessor.changeLaneAction.started += changeLaneResponse.RecieveInput;
    }

    private void UnsubscribeResponses()
    {
        inputProcessor.jumpAction.started -= jumpResponse.RecieveInput;
        inputProcessor.dashAction.started -= dashResponse.RecieveInput;
        inputProcessor.dashAction.canceled -= dashResponse.EndInput;
        inputProcessor.changeLaneAction.started -= changeLaneResponse.RecieveInput;
    }

    public void MoveAction() 
    {
        gameplayManager.Move(xAxis);
    }     

    public void MoveAction(ChargingDashPlayerState _currentState) 
    {
        gameplayManager.Move(xAxis * gameplayVariables.chargingDashSpeedMultiplier);
    }   

    public void StartCoyoteTime()
    {
        StartCoroutine(CoyoteTimeCoroutine());
    }

    private IEnumerator CoyoteTimeCoroutine()
    {
        yield return new WaitForSeconds(gameplayVariables.coyoteTime);
        stateMachine.currentState.StateTimeout(GetState("running"));
    }


}

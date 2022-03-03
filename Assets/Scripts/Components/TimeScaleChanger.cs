using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeScaleChanger : MonoBehaviour
{
    public float slowMotionMultiplier = 0.5f;
    private InputProcessor inputProcessor;


    private void Awake() 
    {


    }
    void Start()
    {
        inputProcessor = GetComponent<PlayerStateMachineBusiness>().inputProcessor;
        inputProcessor.timeScaleAction.started += ToggleTimeScale;
    }

    private void ToggleTimeScale(InputAction.CallbackContext _context)
    {
        if(Time.timeScale == 1f)
            Time.timeScale = slowMotionMultiplier;
        else
            Time.timeScale = 1f;
    }
}

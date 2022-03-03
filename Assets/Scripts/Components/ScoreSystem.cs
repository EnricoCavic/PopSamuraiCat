using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private FloatObject score;

    private void Awake() 
    {
        score.value = 0;    
    }

    private void FixedUpdate() 
    {
        score.value += 1;        
    }

}

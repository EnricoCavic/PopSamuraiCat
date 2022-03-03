using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreDisplay : MonoBehaviour
{
    [SerializeField] private FloatObject previousScore;
    private Text textComponent;
    private static float highScore = 0f;
    private void Awake() 
    {
        textComponent = GetComponent<Text>();
        if(previousScore.value > highScore)
        {
            highScore = previousScore.value;
        }
        textComponent.text = highScore.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatReaderUI : MonoBehaviour
{
    [SerializeField] private FloatObject displayValue;
    private Text textComponent;

    private void Start() 
    {
        textComponent = GetComponent<Text>();
        textComponent.text = displayValue.value.ToString();
    }

    private void FixedUpdate() 
    {
        textComponent.text = displayValue.value.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputData 
{
    public string actionName;
    public int frameCount;
    public InputData(string _actionName, int _frameCount)
    {
        actionName = _actionName;
        frameCount = _frameCount;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Float Object", menuName = "ScriptableObjects/FloatObject")]

public class FloatObject : ScriptableObject
{
    public float value;
    [NonSerialized] public float unserializedValue;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Level Part", menuName = "ScriptableObjects/Level Part")]
public class LevelPartObject : ScriptableObject
{
    public GameObject prefab;
    [NonSerialized] public GameObject prefabInstance;
    public float width;
    public int partDifficulty;
    public Vector3 endPosition => prefabInstance.transform.position + new Vector3(width, 0, 0);
}

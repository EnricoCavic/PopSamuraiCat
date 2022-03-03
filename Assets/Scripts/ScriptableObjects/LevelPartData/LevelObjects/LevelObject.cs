using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "ScriptableObjects/LevelData")]
public class LevelObject : ScriptableObject
{
    public List<LevelPartObject> parts;
    public bool allowRepeatingParts = false;
}

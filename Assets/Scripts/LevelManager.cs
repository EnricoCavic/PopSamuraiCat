using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelObject level;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 startingPosition;
    private LevelPartObject lastLevelPart;
    [SerializeField] private float spawnDistance = 30f;


    private void Start() 
    {
        transform.position = startingPosition;
        SpawnPart();
    }

    private void Update() 
    {
        if(Vector3.Distance(playerTransform.position, lastLevelPart.endPosition) < spawnDistance)
        {
            SpawnPart();
        }
    }

    private void SpawnPart()
    {       
        if(level.parts == null)
            return;

        LevelPartObject nextPart = level.parts[Random.Range(0, level.parts.Count)];

        if(nextPart == lastLevelPart && !level.allowRepeatingParts)
        {
            SpawnPart();
            return;
        }

        lastLevelPart = nextPart;
        lastLevelPart.prefabInstance = Instantiate(lastLevelPart.prefab, transform.position, Quaternion.identity);
        transform.position = lastLevelPart.endPosition;
    }

}

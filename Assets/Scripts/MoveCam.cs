using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    [SerializeField] private float velocidade = 3f;
    private Vector3 moveVector;

    private void Start() 
    {
     moveVector = new Vector3(velocidade, 0, 0);    
    }
    void Update()
    {
        transform.position += moveVector;
    }
}

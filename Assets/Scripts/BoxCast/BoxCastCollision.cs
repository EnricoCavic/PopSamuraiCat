using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BoxCastCollision
{
    public string indentificador;
    public Transform origin;
    public Vector3 direction;
    public float distanciaOrigem;
    public float offsetOrigem;
    public Vector3 modificadorEscala;
    public LayerMask collisionLayer;
    public bool toggleDraw;
    private Vector3 multiplicadorPos => -direction * offsetOrigem ;
    public Vector3 posFinal => origin.position + multiplicadorPos;
    public Vector3 escalaFinal => origin.localScale + modificadorEscala;
    public Vector3 dirMultiplicada => direction * distanciaOrigem;


    public BoxCastCollision()
    {

    }

    public bool ChecarOverlap()
    {
        bool boxCast = Physics.BoxCast(posFinal, escalaFinal/2 , direction, origin.rotation, distanciaOrigem, collisionLayer);

        return boxCast;
    }

    public void DrawCollision()
    {
        if(ChecarOverlap())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireCube(posFinal + dirMultiplicada, escalaFinal);

    }



}

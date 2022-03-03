using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCastManager : MonoBehaviour
{
    public List<BoxCastCollision> listaCasts = new List<BoxCastCollision>();

    public bool ChecarBoxCast(string _identificador)
    {
        foreach (BoxCastCollision _cast in listaCasts)
        {   
            if(_cast.indentificador == _identificador)
                return _cast.ChecarOverlap();

        }           

        return false;
    }

    private void OnDrawGizmos() 
    {
        foreach (BoxCastCollision _cast in listaCasts)
        {   
            if(_cast.toggleDraw && _cast.origin != null)
                _cast.DrawCollision();
        }           

    }

}

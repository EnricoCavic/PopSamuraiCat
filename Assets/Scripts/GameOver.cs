using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private BoxCollider myCollider;
    private Vector3 coliderCenter;
    private bool canRestart = false;
    private WaitForSeconds wait;
    private SceneChanger sceneChanger;

    private void Awake() 
    {
        myCollider = GetComponent<BoxCollider>();
        sceneChanger = GetComponent<SceneChanger>();


    }

    private void Start() 
    {
        wait = new WaitForSeconds(1f);
        canRestart = false;
        StartCoroutine(WaitToRestart());
    }

    private IEnumerator WaitToRestart()
    {
        yield return wait;
        canRestart = true;
    }

    private void LateUpdate() 
    {
        coliderCenter = new Vector3(myCollider.center.x, myCollider.center.y, transform.position.z * -1);
        myCollider.center = coliderCenter;
    }

    private void OnTriggerEnter(Collider _other) 
    {
        if(_other.tag == "Player" && canRestart)
            sceneChanger.ChangeScene();

    }


}

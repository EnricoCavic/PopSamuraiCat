using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextActivationControl : MonoBehaviour{
    public Button ButtonControls;
    public GameObject controlOption;
    public GameObject dactivate;
    void Start(){
        ButtonControls.onClick = new Button.ButtonClickedEvent();
        ButtonControls.onClick.AddListener(() => controllerText());
    }
    public void controllerText(){
        controlOption.gameObject.SetActive(!controlOption.gameObject.activeInHierarchy);
        dactivate.gameObject.SetActive(false);
    }
}

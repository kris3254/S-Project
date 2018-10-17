using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermitirAnimaciones : MonoBehaviour {

    public playerControllerCustom pController;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartedAnimation()
    {
        pController.canDoThings = false;
    }

    public void FinishedAnimation()
    {
        Debug.Log("Hola");
        pController.canDoThings = true;
    }
}

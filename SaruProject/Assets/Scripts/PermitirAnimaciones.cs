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
        pController.canDoThings = true;
    }

    /* 
     * En pController Input L1 añadir canDoThings == true 
     * 
     if (pController.modoGuardian) {
						pController.cambiandoModo = true;
						Invoke ("AnimDelay", delayIfGuardianMode);
					} else {
						AnimDelay ();
					}

    void AnimDelay()
    {
        pController.canDothings = true ? false : true;
    }

    */
}

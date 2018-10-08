using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorMusicaEpicaCayado : MonoBehaviour {

    private bool isFirstTime=true;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            if(isFirstTime)
            {
                AudioManager.instance.PlaySound("MusicaEpicaCayado");
                isFirstTime = false;
            }
        }
    }
}

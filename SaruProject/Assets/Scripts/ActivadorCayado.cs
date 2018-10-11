using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorCayado : MonoBehaviour {

    public GameObject cayadoPlayer;
	[SerializeField]
	playerControllerCustom pController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            gameObject.SetActive(false);
            AudioManager.instance.StopSound("MusicaEpicaCayado"); 
            AudioManager.instance.PlaySound("RecogerColeccionable");
            Invoke("ActivarCayadoPlayer", 2);
        }
    }

    void ActivarCayadoPlayer()
    {
        cayadoPlayer.SetActive(true);
		pController.canAttack = true;
    }
}

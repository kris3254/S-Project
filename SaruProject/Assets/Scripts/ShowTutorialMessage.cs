using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Este script lo contiene un trigger puesto en la zona específica donde deseamos que se cargue el mensaje tutorial en la UI para mostrarle algo al usuario.
public class ShowTutorialMessage : MonoBehaviour {

    private bool isFirstTime = true;
	public string[] tutorialMessagesToShow;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") && (isFirstTime))
        {
			foreach (string dialog in tutorialMessagesToShow) {
				UIManager.instance.AddDialogText (dialog);
			}
            //UIManager.instance.ShowTutorialMessage(tutorialMessagesToShow);
            isFirstTime = false;    
        }
    }
}

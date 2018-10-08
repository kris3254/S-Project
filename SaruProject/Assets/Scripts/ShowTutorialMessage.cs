using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Este script lo contiene un trigger puesto en la zona específica donde deseamos que se cargue el mensaje tutorial en la UI para mostrarle algo al usuario.
public class ShowTutorialMessage : MonoBehaviour {

    public GameObject[] tutorialMessagesToShow;//array donde arrastro cada uno de los gameObjects desactivados (mensajes, que contienen tanto texto como imagenes (activados)) contenidos en un gameObject padre (mensaje total, activado), que forman un mensaje completo
                                                //hay tutoriales que solo contienen 1 mensaje y otros que contienen varios. Estos mensajes se encuentran el la escena dentro de UI=>Tutorial Panel  
    private bool isFirstTime = true;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") && (isFirstTime))
        {
            UIManager.instance.ShowTutorialMessage(tutorialMessagesToShow);
            isFirstTime = false;    
        }
    }
}

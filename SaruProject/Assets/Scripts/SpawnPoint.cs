using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Este script es un componente del objeto Checkpoint (que contiene un gameObject vacio con la transform correspondiente a la posicion de respawneo)
//Una vez que entre en su radio de accion tengo que llamar al LevelManager y actualizar la posicion de Respawn del Player
//ademas, tambien se iluminaria la luz del checkpoint (indica interactividad), y se regenera la vida a su totalidad (3)
public class SpawnPoint : MonoBehaviour
{
    public Transform playerSpawnTransform;
    public GameObject checkPointLight;//desactivarla en la escena
    public GameObject sphereCheckpoint;//Desactivarla en la escena
    private bool isFirstTime;


    private void Start()
    {
        isFirstTime = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player"))
        {
            AudioManager.instance.PlaySound("CheckPointSound");
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "Player") && (!other.gameObject.GetComponent<playerControllerCustom>().modoGuardian) && ((Input.GetButtonDown("Triangle_PS4")) ) )
        {
            AudioManager.instance.PlaySound("ActivarCheckpoint");
            sphereCheckpoint.gameObject.SetActive(true);
            checkPointLight.gameObject.SetActive(true);
            //Iluminar la lucecita del checkpoint
            LevelManager.instance.UpdateSpawnPoint(playerSpawnTransform);
            if (isFirstTime)
                PlayerManager.instance.SetHealth(3);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        isFirstTime = false;
        AudioManager.instance.StopSound("CheckPointSound");
    }
     

}

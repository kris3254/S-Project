using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenInteractable : MonoBehaviour {

    public GameObject triangleInteractable;
    public GameObject[] dialogTexts;
    private bool isOnTrigger = false;


    private void Update()
    {
        if (isOnTrigger && ((Input.GetKeyDown(KeyCode.T)) || (Input.GetButtonDown("Triangle_PS4"))) )
        {
            Debug.Log("El player ha entrado en el area del ciudadano y ha pulsado la tecla de dialogo");
            UIManager.instance.ShowDialog(dialogTexts);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            Debug.Log("El player ha entrado en el area del ciudadano");
            triangleInteractable.gameObject.SetActive(true);
            isOnTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("El player ha salido del area del ciudadano");
            triangleInteractable.gameObject.SetActive(false);
            isOnTrigger = false;
        }
    }
}

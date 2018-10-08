using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Este script lo va a contener un item que será recogido por el player, al entrar en su collider, se desactiva el objeto (lo recoge), y desde el UIManager mostramos
//el concept asociado como recompensa por haber recogido el item con su efecto de audio asociado.
public class Coleccionable : MonoBehaviour {

    public Sprite conceptImage;//concept recompensa a mostrar al usuario
    private Color conceptBackgroundColor = new Color(255, 255, 255, 255);

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            UIManager.instance.conceptPanel.GetComponent<Image>().color = conceptBackgroundColor;
            UIManager.instance.ShowConcept(conceptImage);
            PlayerManager.instance.collectablesList.Add(conceptImage);
            gameObject.SetActive(false);
        }
        
    }

   
}

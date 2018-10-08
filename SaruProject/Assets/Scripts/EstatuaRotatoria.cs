
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que modela el comportamiento de la estatua rotatoria (solo 1 sistema de particulas, en vez de 2) que escupe fuego.
//La estatua escupe fuego hacia su forward, pero va rotando formando un ángulo determinado (cuando termina de rotar espera un tiempo y vuelve a repetir esto) (quita 1/3 de vida al player)
//el script va incluido en el gameObject cabeza de la estatua rotatoria, ademas de este script añadir un collider isTriggered. El sistema de particulas es hijo de esta. 


public class EstatuaRotatoria : MonoBehaviour
{
    public ParticleSystem sistemaParticulasEstatua;//el sitema de particulas tiene que ser hijo del gameObject que contiene este script (la cabeza de la estatua rotatoria).
    public float rotationSpeed;
    private AudioSource audioSourceFire;//fuente de audio que contiene el clip de sonido de fuego o de energía (para modo 1)




    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            StartCoroutine(StatueModeX(rotationSpeed));
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StopAllCoroutines();
            sistemaParticulasEstatua.Stop();//para corregir el bug consistente en que si en el momento en el que salgo del trigger y para todas las corrutina, el sistema de particulas
                                             //esta activado, nunca se desactivan, y queremos que al salir siempre se desactiven ademas de detener la ejecucion de las corrutinas
        }

    }

    //Modo X=> La estatua escupe fuego hacia su forward, pero va rotando formando un ángulo determinado (cuando termina de rotar espera un tiempo y vuelve a repetir esto).
    IEnumerator StatueModeX(float spinSpeed)
    {
        sistemaParticulasEstatua.Play();
        while (true)
        {
            Debug.Log("Estoy rotando");
            gameObject.transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

    }
}
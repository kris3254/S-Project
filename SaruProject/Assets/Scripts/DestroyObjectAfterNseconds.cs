using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Segun se activa un gameObject que contiene este script, a los secondsToStartDeactivation se desactiva
public class DestroyObjectAfterNseconds : MonoBehaviour
{

    public float secondsToStartDeactivation;// segundos de vida del objeto desde que este se inicializa (se ejecuta el start), transcurridos estos, el objeto se destruye


    // Use this for initialization
    void Start()
    {
        StartCoroutine(DestroyElementAfterNseconds(secondsToStartDeactivation));
    }

    IEnumerator DestroyElementAfterNseconds(float n)
    {
        yield return new WaitForSeconds(n);
        Destroy(gameObject);
    }
}


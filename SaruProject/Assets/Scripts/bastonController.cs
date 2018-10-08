using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bastonController : MonoBehaviour {

    public GameObject ballDeath;
    public playerControllerCustom controller;
    public Transform puntaBaston;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" )
        {
            if (controller.isAttacking)
            {
                Instantiate(ballDeath, puntaBaston.position, Quaternion.identity);
                Destroy(other.gameObject,0.1f);
            }
        }
    }
}

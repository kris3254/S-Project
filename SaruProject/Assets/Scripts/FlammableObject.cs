using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableObject : MonoBehaviour {

    public float firingTime;// tiempo que el objeto inflamable contra el que choca el proyectil va a estar ardiendo antes de desaparecer
    public ParticleSystem[] ParticlesArray;//conjunto de particulas de fuego que el objeto inflamable inflamable que contiene este script ha de contener
                                           //han de estar todas con el playOnAwake desactivado 
    public GameObject particleSmoke;
    public bool isFinalDemoTree;
    public DemoEnd demoEnd;

    public playerControllerCustom pController;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "FireBall")// el objeto contra el que choca es una bola de fuego
        {
            Debug.Log("Colision con objeto inflamable detectada");
            AudioManager.instance.PlaySound("EstatuaFuego");
            Destroy(other.gameObject);//Destruimos la bola de fuego segun detectamos la colision con el objeto inflamable
            StartCoroutine("EsperarDestruccion");
        }
    }

    IEnumerator EsperarDestruccion()
    {
        foreach (ParticleSystem p in ParticlesArray)
        {
            p.Play();
        }
        yield return new WaitForSeconds(firingTime);
        particleSmoke.SetActive(true);
        AudioManager.instance.StopSound("EstatuaFuego");

        if (isFinalDemoTree)
        {
            yield return new WaitForSeconds(0.5f);
            pController.canDoThings = false;
            pController.anim.SetFloat("speed", 0f);
            demoEnd.StartFade();
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        
    }
}

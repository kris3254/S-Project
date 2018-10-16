using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttackController : MonoBehaviour {

	public float speed;//velocidad de desplazamiento del proyectil
    public float firingTime;// tiempo que el objeto inflamable contra el que choca el proyectil va a estar ardiendo antes de desaparecer
    public float lifeTime;//Tiempo de vida del proyectil de fuego
    private GameObject flammableObject;//El objeto contra el que choca ha de tener tag de inflamable si queremos que arda, y un particle System como componente asociado


	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.forward * speed);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Flammable")// el objeto contra el que choca tiene que tener el tag de objeto inflamable
        {
            Debug.Log("Colision con objeto inflamable detectada");

            flammableObject = other.gameObject;
            //flammableObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine("EsperarDestruccion");
        }
    }

    IEnumerator EsperarDestruccion()
    {
        yield return new WaitForSeconds(firingTime);
        Destroy(flammableObject);
        Destroy(gameObject);
    }
}

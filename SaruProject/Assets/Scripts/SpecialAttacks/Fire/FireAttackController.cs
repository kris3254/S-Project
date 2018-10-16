using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttackController : MonoBehaviour {

	public float speed;//velocidad de desplazamiento del proyectil
    public int lifetime;//tiempo de vida de la bola de fuego (usado para controlar el que siempre se destruya la bola de fuego aunk no impacte con un objeto inflamable
                        // si lo hace, se destruye automaticamente, sino pasado este lifetime
    private Rigidbody rigid;


    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        AudioManager.instance.PlaySound("EstatuaFuego");
        StartCoroutine("WaitForDie");
    }

    // Update is called once per frame
    void Update () {

        rigid.AddForce(gameObject.transform.forward * speed * Time.deltaTime);
	}

    IEnumerator WaitForDie()
    {
        yield return new WaitForSeconds(lifetime);
        AudioManager.instance.StopSound("EstatuaFuego");
        Destroy(gameObject);
    }
}

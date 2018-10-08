using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que modela el comportamiento de un miniorbe, nada mas cargarse la escena, cada uno incrementa la variable del levelManager que contiene la cantidad total
//de miniorbes del nivel. Además, al recogerlos el player se actualiza el num de miniorbes que el player ha recogido hasta ese momento en el nivel.
public class Miniorbe: MonoBehaviour {

	// Use this for initialization
	void Start () {
        LevelManager.instance.AddMiniOrbToTotalCount();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            LevelManager.instance.AddMiniOrbToPlayerCount();
            AudioManager.instance.PlaySound("RecogerMiniorbe");
            Destroy(gameObject);
        }

    }
}

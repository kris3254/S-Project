using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateFire : MonoBehaviour {

	public SpecialAttacks saruSpecialAttacks;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if((other.gameObject.tag=="Player"))
		{
			//Se activa el uso del poder de fuego
			saruSpecialAttacks.ActivateFire();
			Destroy (this);
		}

	}
}

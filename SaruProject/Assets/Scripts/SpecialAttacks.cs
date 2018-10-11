using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttacks : MonoBehaviour {

	public enum attackType{
		Fire,
		Air
	}

	public attackType actualType;

	public bool canUseSpecialAttack = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!canUseSpecialAttack)
			return;

		//Debug.Log ("Prueba -> "+Input.GetAxis ("R2_PS4")	);
		if (Input.GetAxisRaw ("R2_PS4") == 1) {

			switch(actualType){

			case attackType.Fire:
				FireAttack ();
				break;
			
			case attackType.Air:
				break;

			}

		}

	}

	public void UseFire(){
		actualType = attackType.Fire;
	}

	void FireAttack(){
	
	}
}

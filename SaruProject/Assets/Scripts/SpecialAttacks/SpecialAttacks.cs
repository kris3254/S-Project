using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttacks : MonoBehaviour {

	public enum attackType{
		Fire,
		Air
	}

	public attackType actualType;

	//El orden viene dado por el orden escrito en attackType
	bool[] canUseSpecificAttack;

	[Header("Fire")]
	public GameObject firePrefab;

	// Use this for initialization
	void Start () {
		//Inicializa al tamaño del numero de tipos de attackType
		canUseSpecificAttack = new bool[System.Enum.GetNames(typeof(attackType)).Length];
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log ("Prueba -> "+Input.GetAxis ("R2_PS4")	);
		if (Input.GetAxisRaw ("R2_PS4") == 1) {
			Debug.Log ("Input bien");
			switch(actualType){

			case attackType.Fire:
				if (canUseSpecificAttack [0]) {
					FireAttack ();
				}
				break;
			
			case attackType.Air:
				break;

			default:
				break;

			}

		}

	}

	public void UseFire(){
		actualType = attackType.Fire;
	}

	void FireAttack(){
		Debug.Log ("Dispara Fuego");
	}

	public void ActivateFire(){
		canUseSpecificAttack [0] = true;
		actualType = attackType.Fire;
	}
}

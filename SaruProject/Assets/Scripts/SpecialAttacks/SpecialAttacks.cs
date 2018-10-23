using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttacks : MonoBehaviour {

	public playerControllerCustom pController;

	public enum attackType{
		Fire,
		Air
	}

	public attackType actualType;

	//El orden viene dado por el orden escrito en attackType
	bool[] canUseSpecificAttack;
	public Transform saruTransform;

	[Header("Fire")]
	public GameObject firePrefab;
	public float timeBetweenFireAttack = 2f;
	public Vector3 offsetDistanceToShoot = new Vector3 (0,1,0);
	public float timeFireAnim = 1;
	public float delayIfGuardianMode = 0.8f;

	// Use this for initialization
	void Start () {
		//Inicializa al tamaño del numero de tipos de attackType
		canUseSpecificAttack = new bool[System.Enum.GetNames(typeof(attackType)).Length];
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log ("Prueba -> "+Input.GetAxis ("R2_PS4")	);
		if (Input.GetAxisRaw ("R2_PS4") == 1 && pController.canDoThings == true) {
			
			switch(actualType){

			case attackType.Fire:
				if (canUseSpecificAttack [(int)attackType.Fire]) {
					canUseSpecificAttack [(int)attackType.Fire] = false;
					if (pController.modoGuardian) {
						pController.cambiandoModo = true;
						Invoke ("FireAttack", delayIfGuardianMode);
					} else {
						FireAttack ();
					}

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
		pController.FireAttackAnim ();

		Invoke ("AttackEffect",timeFireAnim);
	}

	void AttackEffect(){
		GameObject fire = Instantiate (firePrefab, saruTransform.position+offsetDistanceToShoot.x*saruTransform.right+offsetDistanceToShoot.y*saruTransform.up+offsetDistanceToShoot.z*saruTransform.forward, saruTransform.rotation) as GameObject;
		StartCoroutine (TimeWait((int)attackType.Fire,timeBetweenFireAttack));
	}

	IEnumerator TimeWait(int typeOfUse, float delay){
		yield return new WaitForSeconds (delay);
		canUseSpecificAttack [typeOfUse] = true;
	}

	public void ActivateFire(){
		canUseSpecificAttack [(int)attackType.Fire] = true;
		actualType = attackType.Fire;
	}

}
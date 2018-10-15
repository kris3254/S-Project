using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttackController : MonoBehaviour {

	public float speed = 0.1f;
	public float timeAlive = 1;

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject,timeAlive);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.forward * speed);
	}
}

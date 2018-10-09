using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestruirObjInflaamable : MonoBehaviour 
{
	GameObject cosa;
	public void Start ()
	{
		cosa = GameObject.Find ("inflamable");
		Destroy (cosa,(float) 1);
		Destroy (this.gameObject,(float)1);
	}
}

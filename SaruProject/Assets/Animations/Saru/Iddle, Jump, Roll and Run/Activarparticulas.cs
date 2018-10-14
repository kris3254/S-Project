using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activarparticulas: MonoBehaviour {

	public ParticleSystem PieDerecho;
	public ParticleSystem PieIzquierdo;

	public void ActivarPieDerecho ()
	{
		PieDerecho.Play();
	}

	public void ActivarPieIzquierdo ()
	{
		PieIzquierdo.Play ();
	}
}		

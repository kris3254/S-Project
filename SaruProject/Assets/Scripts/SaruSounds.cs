using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaruSounds : MonoBehaviour {

    public playerControllerCustom controller;

	public void PasoDerecho()
    {
        int num = Random.Range(0, 1);

        if(num==1)
        {
            AudioManager.instance.PlaySound("PasoPiedra1");
        }
        else
        {
            AudioManager.instance.PlaySound("PasoPiedra2");
        }
    }

    public void PasoIzquierdo()
    {
        int num = Random.Range(0, 2);

        if (num == 1)
            AudioManager.instance.PlaySound("PasoPiedra3");
        else
            AudioManager.instance.PlaySound("PasoPiedra4");
    }

    public void Rodar()
    {
        Debug.Log("Evento rodar ejecutado");
        AudioManager.instance.PlaySound("Rodar");

    }

    public void SaltoS()
    {
        Debug.Log("Evento salto ejecutado");
        int num = Random.Range(0, 2);

        if (num == 1)
            AudioManager.instance.PlaySound("Salto1");
        else
            AudioManager.instance.PlaySound("Salto2");
    }

    public void MoverCayado()
    {
        int num = Random.Range(0, 2);

        if (num == 1)
            AudioManager.instance.PlaySound("MovCayado1");
        else
            AudioManager.instance.PlaySound("MovCayado2");
    }

    public void GolpearCayado()
    {
        int num = Random.Range(0, 2);

        if (num == 1)
            AudioManager.instance.PlaySound("GolpeCayado1");
        else
            AudioManager.instance.PlaySound("GolpeCayado2");
    }
    public void AttackAnimationEnd()
    {
        controller.isAttacking = false;
    }
}

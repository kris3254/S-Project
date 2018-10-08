﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//este script lo tiene atachado el propio player del juego
public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance = null;
    public float timeToTakeHealthWithParticles;//valor de tiempo que tenemos que estar en contacto con el sistema de particulas de fuego para perder un punto de vida
    
    public int health;//numero de vidas del player
    private bool playerIsHittedWithParticles;//booleano para garantizar que unicamente se nos quita un punto de vida por cada tiempo especificado aunk sigamos en contacto con el sistema de particulas
    public playerControllerCustom controller;

    [HideInInspector]
    public List<Sprite> collectablesList;//Conjunto de imagenes de los coleccionables que el jugador ha descubierto hasta ese momento.

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        health = 3;
        playerIsHittedWithParticles = true;
        collectablesList = new List<Sprite>();
	}


    public void DecrementHealth(int healthToLose)
    {
        health -= healthToLose;

        int num = Random.Range(0, 2);

        if (num == 1)
            AudioManager.instance.PlaySound("RecibirDanio1");
        else
            AudioManager.instance.PlaySound("RecibirDanio2");

        UIManager.instance.CameraShake();
        //audioSourcePlayer.clip = losingHealthSound;
        //audioSourcePlayer.Play();
        if (health<=0)
        {
            controller.isAttacking = false;
            controller.isRolling = false;
            health = 0;
            Debug.Log("El player ha muerto");
            UIManager.instance.UpdateHealthHUD();//actualizamos el hud con el nuevo valor de vidas del player
            LevelManager.instance.Respawn();
            UIManager.instance.FadeInOutEffect();
        }
        else
        UIManager.instance.UpdateHealthHUD();//actualizamos el hud con el nuevo valor de vidas del player
       

    }

    //Este metodo no tiene sentido de momento (la vida solo se resetea a 3 cuando respawneamos) o cuando atravesamos un checkpoint
    public void IncrementHealth(int healthToIncrement)
    {
        health+=healthToIncrement;

        if (health > 3)
        {
            health = 3;
        }
        UIManager.instance.UpdateHealthHUD();
        //audioSourcePlayer.clip = winningHealthSound;
        //audioSourcePlayer.Play();
       
    }

    
    public int GetHealthValue()
    {
        return health;
    }

    public void SetHealth(int h)
    {
        health = h;
        UIManager.instance.UpdateHealthHUD();
    }


    //Metodo que se ejecuta cuando cualquier sistema de particulas colisiona con el player.
    //Si este metodo se incluye en un script atachado al objeto contra el que la particula colisiona, other es una referencia al sistema de particulas que causa la colision
    //Si este metodo se incluye en un script atachado al sistema de particulas, el parametro other es el objeto contra el que las particulas colisionan (tiene que tener un collider).
    //Lo primero que tenemos que hacer por tanto es comprobar que particulas son las que nos han impactado (las de las estatuas u otras)
    private void OnParticleCollision(GameObject other)
    {
        //if (other.gameObject.name=="ParticulasEstatua")
        if(playerIsHittedWithParticles)
        {
            Debug.Log("Las particulas estan colisionando con el player");
            //el sistema de particulas puede pertenecer a una estatua doble (fuego o cono luz) o una rotatoria
            EstatuaRotatoria estatuaRotatoria = other.GetComponentInParent<EstatuaRotatoria>();
            EstatuaDoble estatuaDoble = other.GetComponentInParent<EstatuaDoble>();

            if (estatuaRotatoria != null)//la particula que me ha impactado pertenece a una estatua rotatoria
            {
                DecrementHealth(1);
                StartCoroutine(DamagePlayerWithParticles(timeToTakeHealthWithParticles));
            }
            else if (estatuaDoble != null && estatuaDoble.mode != 1)//estatua doble en modo fuego (cualquiera menos el modo 1)
            {
                DecrementHealth(1);
                StartCoroutine(DamagePlayerWithParticles(timeToTakeHealthWithParticles));
            }

            else //estatua doble en modo luz (modo 1), el player muere con solo recibir un hit
            {
                DecrementHealth(3);
                StartCoroutine(DamagePlayerWithParticles(timeToTakeHealthWithParticles));

            }

        }
    }

    //Esta corrutina modela el hecho de que las particulas no me quiten mas de un punto de vida por cada valor de tiempo de la variable timeToTakeHealthWithParticles
    IEnumerator DamagePlayerWithParticles(float time)
    {
        playerIsHittedWithParticles = false;
        yield return (new WaitForSeconds(time));
        playerIsHittedWithParticles = true;
    }

    

   





}

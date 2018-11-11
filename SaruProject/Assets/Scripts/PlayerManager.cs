using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System;

//este script lo tiene atachado el propio player del juego
public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance = null;
    public float timeToTakeHealthWithParticles;//valor de tiempo que tenemos que estar en contacto con el sistema de particulas de fuego para perder un punto de vida
    
    public int health;//numero de vidas del player
    public bool playerIsHittedWithParticles;//booleano para garantizar que unicamente se nos quita un punto de vida por cada tiempo especificado aunk sigamos en contacto con el sistema de particulas
    public playerControllerCustom controller;
    public float timeToRespawn = 0.5f;
    public CinemachineFreeLook camera;

    [HideInInspector]
    public List<Sprite> collectablesList;//Conjunto de imagenes de los coleccionables que el jugador ha descubierto hasta ese momento.

    public event EventHandler ShakeCamera;
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
        if (!controller.stop)
        {
            Debug.Log("Recibo daño");
            health -= healthToLose;
            ShakeCamera(this,new EventArgs());

            int num = UnityEngine.Random.Range(0, 2);

            if (num == 1)
                AudioManager.instance.PlaySound("RecibirDanio1");
            else
                AudioManager.instance.PlaySound("RecibirDanio2");

            UIManager.instance.CameraShake();

            //audioSourcePlayer.clip = losingHealthSound;
            //audioSourcePlayer.Play();
            UIManager.instance.UpdateHealthHUD();//actualizamos el hud con el nuevo valor de vidas del player
            if (health <= 0)
            {
                controller.isAttacking = false;
                controller.isRolling = false;
                controller.cambiandoModo = false;
                controller.isDead = true;
                controller.stop = true;
                health = 0;
                Debug.Log("El player ha muerto");
                StartCoroutine(RespawnLevelAfterDeath(timeToRespawn));

            }       
        }
    }

    public void InstantDead()
    {
        int num = UnityEngine.Random.Range(0, 2);

        if (num == 1)
            AudioManager.instance.PlaySound("RecibirDanio1");
        else
            AudioManager.instance.PlaySound("RecibirDanio2");

        health = 0;
        Debug.Log("El player ha muerto por Instant Dead");
        UIManager.instance.UpdateHealthHUD();//actualizamos el hud con el nuevo valor de vidas del player
        LevelManager.instance.Respawn();
        UIManager.instance.FadeInOutEffect();
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

    IEnumerator RespawnLevelAfterDeath(float time)
    {
        yield return new WaitForSeconds(time);
        controller.isDead = false;
        UIManager.instance.UpdateHealthHUD();//actualizamos el hud con el nuevo valor de vidas del player
        LevelManager.instance.Respawn();
        UIManager.instance.FadeInOutEffect();
        playerIsHittedWithParticles = true;
    }

    //este metodo hace que el modelo de saru mire a una posicion
    public void LookAtPosition(Vector3 lookAt)
    {
        Vector3 relativePos = lookAt - controller.playerModel.transform.position;
        controller.playerModel.transform.LookAt( lookAt);
        controller.playerModel.transform.localRotation = new Quaternion(0f, controller.playerModel.transform.localRotation.y, 0f, controller.playerModel.transform.localRotation.w);   
    }


    public void SetCameraPosition(Vector2 value)
    {
        camera.m_XAxis.Value = value.x;
        camera.m_YAxis.Value = value.y;
    }

    public Vector2 GetCameraPosition()
    {
        return new Vector2(camera.m_XAxis.Value, camera.m_YAxis.Value);
    }

    //borrar de aqui  pa abajo
    private void Update()
    {
        //para optener la posicion que queremos de en la camara
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log( " // X Value: " +  camera.m_XAxis.Value.ToString() + " // Y Value: " + camera.m_YAxis.Value.ToString());
        }

        //para quitar puntos de vida
        if (Input.GetKeyDown(KeyCode.M))
        {
            DecrementHealth(1);
        }
    }






}

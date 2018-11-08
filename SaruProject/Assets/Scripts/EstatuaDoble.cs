
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que modela el comportamiento de la estatua doble que escupe fuego.
//Este script se incluye como componente de la propia estatua.
//Tambien hemos de añadir un box collider y darle el tamaño que consideremos (al entrar el player en el collider, se activan las estatuas).
//se observan varios comportamientos distintos (Modos de funcionamiento)
//Modo 0=> La estatua escupe fuego en línea recta (dos chorros) a intervalos constantes de tiempo (escupe, pausa, escupe...) (quita 1/3 de vida al player)
//Modo 1=> La estatua proyecta dos chorros de luz a intervalos intermitentes (igual que el fuego), con la diferencia que en este caso, si los conos de luz te impactan , te petrificas (mueres, quita 3/3 vida al player)
//Modo 2=> En este caso dos estatuas se situan paralelas mirandose la una a la otra, en el centro de estas hay una piedra, cuando la una lanza los dos chorros la otra no (y viceversa) obligando al player a elegir un lado
//Para lograr este efecto ponemos una en modo 0 y la de enfrente en modo 2
//Modo 3=> Solo esta activado el chorro de abajo (para forzar saltar al jugador)
//Modo 4=> Solo esta activado el chorro de arriba (para forzar el rodar al jugador)
//Modo 5=> Se va alternando el chorro de arriba con el chorro de abajo con un parámetro de tiempo variable


public class EstatuaDoble : MonoBehaviour
{
    public int mode;// modo de funcionamiento de la estatua
    public float firingTimeMode012;// variable que regula cada cuanto tiempo escupe fuego la estatua en modo 0 1 y 2
    public float breakTimeMode012;//variable que recoje el tiempo que la estatua no escupe fuego en modo 0 1 y 2
    public float firingTimeMode5Up;//variable que regula cada cuanto tiempo escupe fuego el chorro superior de la estatua en modo 5
    public float firingTimeMode5Down;//variable que regula cada cuanto tiempo escupe fuego el chorro inferior de la estatua e modo 5
    public ParticleSystem sistemaParticulasUp;//sistema de particulas 1 (superior) que contiene el efecto de fuego o el cono de luz (si la estatua es de las de luz, tendra el prefab del sistema de particulas de luz, si es de las de fuego, tendra el prefab de fuego)
    public ParticleSystem sistemaParticulasDown;//sistema de particulas 2 (inferior) que contiene el efecto de fuego o el cono de luz (si la estatua es de las de luz, tendra el prefab del sistema de particulas de luz, si es de las de fuego, tendra el prefab de fuego)
    public playerControllerCustom saruController;
    public bool saruCanSee = false;

    
    private void Update()
    {
        if (!saruCanSee)
        {
            sistemaParticulasDown.GetComponent<ParticleSystemRenderer>().enabled = saruController.modoGuardian;
            sistemaParticulasUp.GetComponent<ParticleSystemRenderer>().enabled = saruController.modoGuardian;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Si dentro de la zona de acción esta Saru lanzo la corrutina acorde al tipo de funcionamiento que desee que haga la estatua
        if ((other.gameObject.tag == "Player"))
        {
            Debug.Log("El player ha entrado en el trigger de la estatua");
            switch (mode)
            {
                case 0: //Modo 0=>lanza dos chorros de fuego en linea recta, para, lanza... (cada colision detectada quita 1 punto de vida)
                    StartCoroutine(StatueMode0And1(breakTimeMode012,firingTimeMode012));
                    break;

                case 1: //Modo 1=> lanza dos chorros de luz en linea recta (si te impacta te petrificas y mueres).
                    StartCoroutine(StatueMode0And1(breakTimeMode012,firingTimeMode012));
                    break;

                case 2: //Modo 2=> En este caso dos estatuas se situan paralelas mirandose la una a la otra, en el centro de estas hay una piedra, cuando la una lanza el fogonazo la otra no (y viceversa)
                    StartCoroutine(StatueMode2(breakTimeMode012,firingTimeMode012));
                    break;

                case 3: //Modo 3=> Solamente esta activado el chorro de abajo (forzamos saltar al jugador)
                    StatueMode3();
                    break;

                case 4: //Modo 4=> Solamente esta activado el chorro de aarriba (forzamos rodar al jugador)
                    StatueMode4();
                    break;

                case 5: //Modo 5=> 
                    StartCoroutine(StatueMode5(firingTimeMode5Up, firingTimeMode5Down));
                    break;

            }
            


        }
    }

    //Si sale Saru de la zona de acción, desactivo todas las corrutinas
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StopAllCoroutines();
            sistemaParticulasUp.Stop();//para corregir el bug consistente en que si en el momento en el que salgo del trigger y para todas las corrutina, el sistema de particulas
                                       //esta activado, nunca se desactivan, y queremos que al salir siempre se desactiven ademas de detener la ejecucion de las corrutinas
            sistemaParticulasDown.Stop();


        }

    }


    //Modo 0=> La estatua escupe fuego (dos chorros) en línea recta a intervalos constantes de tiempo (escupe, pausa, escupe...)
    //Modo 1=> lanza dos conos de luz en linea recta (si te impacta te petrificas y mueres).
    //n es el tiempo de espera, m es el tiempo que esta activa escupiendo fuego
    IEnumerator StatueMode0And1(float n, float m)
    {
        while (true)
        {
            sistemaParticulasUp.Play();
            sistemaParticulasDown.Play();
            yield return (new WaitForSeconds(m));
            sistemaParticulasUp.Stop();
            sistemaParticulasDown.Stop();
            yield return (new WaitForSeconds(n));
        }
    }



    //Modo 2> Misma funcionalidad que en modo 0, pero invertida, por lo que la corrutina lanza los chorros de fuego y espera en vez de esperar y lanzar chorro (logramos que emitan en oposicion de fase)
    IEnumerator StatueMode2(float n, float m)
    {
        while (true)
        {
            sistemaParticulasUp.Stop();
            sistemaParticulasDown.Stop();
            yield return (new WaitForSeconds(n));
            //audioSourceFire.Play();
            sistemaParticulasUp.Play();
            sistemaParticulasDown.Play();
            yield return (new WaitForSeconds(m));
        }
    }

    //Modo 3=> Solamente esta activado el chorro de abajo (forzamos saltar al jugador)
    void StatueMode3()
    {
        sistemaParticulasDown.Play();
    }

    //Modo 4=> Solamente esta activado el chorro de arriba (forzamos rodar al jugador)
    void StatueMode4()
    {
        sistemaParticulasUp.Play();
    }

    //Modo 5=> Se va alternando el chorro de arriba con el chorro de abajo con un parámetro de tiempo variable
    // n=> firingTimeMode5Up m=> firingTimeMode5Down
    IEnumerator StatueMode5(float n, float m)
    {
        while (true)
        {
            //audioSourceFire.Play();
            sistemaParticulasUp.Play();
            sistemaParticulasDown.Stop();
            yield return (new WaitForSeconds(n));
            sistemaParticulasUp.Stop();
            sistemaParticulasDown.Play();
            yield return (new WaitForSeconds(m));
            
        }
    }



   





}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Script que modela la mecanica de Saru consistente en generar plataformas ocultas.
//Este script ha de ponerse dentro del gameobject runa (dentro de uno vacio que contiene a esta, el sistema de particulas y las plataformas de esa zona
//la runa tiene el collider asociado), al entrar en su trigger, las particulas aparecen y son la guia visual del usuario para que este active la mecánica
//En el sistema de particulas desactivar el script CFX_Autodestruct_Shuriken, y desactivar playOnAwake.
//Añadir como componente a la runa que contiene este script, un trigger y un audioSource con el audio del efecto de generacion de plataformas
//Al activar la mecánica en una región (donde hay 10 plataformas) se van activando secuencialmente de una en una con un tiempo variable. Una vez activadas todas, cada N seg se van destruyendo (de principio a fin)
//la mecánica se activa al presionar un botón. Mientras esté en el trigger , el jugador puede ejecutar la mecanica cuantas veces quiera (reiniciandose todas las plataformas desde el principio), pero en el momento en el que abandona el trigger (comienza a saltarlas) no se le permite reiniciarlas (usar la mecanica de nuevo)
//Este script lo va a llevar un gameObject vacio con un collider, que va a estar situado al inicio de la zona de plataformas.
//Además, en el momento inicial de activacion de la mecánica se reproduce un audio que emula un rayo mágico.


public class PlataformasOcultas : MonoBehaviour
{
    public GameObject[] platformsArray;// array que contiene la totalidad de plataformas ocultas de la region (De 0 a N de plataforma inicial a final en el sentido de recorrido de Saru)
    public float platformDestroyTime;// variable que regula cada cuanto tiempo se van destuyendo las plataformas secuencialmente
    public float platformActivationTime;//variable que recoje el tiempo que transcurre entre que se activa una plataforma y la siguiente
    public ParticleSystem runeParticlesEffect;//sistema de particulas que acompaña al modelo 3D de la runa

    private bool isInTrigger;//variable que regula que mientres este en el trigger de inicio pueda activar la mecanica de las plataformas las veces que quiera (reseteandolas desde el principio), pero en el momento que lo abandono no puedo resetear las plataformas
    private int numberOfPlatforms;// numero de plataformas de la region
    private AudioSource audioSourceMagic;//fuente de audio que contiene el clip de sonido del efecto magico al generar las plataformas



    // Use this for initialization
    void Start()
    {
        isInTrigger = false;
        numberOfPlatforms = platformsArray.Length;
        audioSourceMagic = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Cada vez que le de a la tecla y este dentro del trigger tiene que activarse la mecanica, es decir, inciarse la corrutina. Solo puede haber una corrutina ejecutandose a la vez
        // por lo que antes de activarla destruyo la que ya está ejecutándose ( en el caso de que le de dos veces a la tecla antes de que la corrutina termine).
        if(isInTrigger && (Input.GetKeyDown(KeyCode.R) ||(Input.GetButtonDown("Fire2"))))
        {
            StopAllCoroutines();
            StartCoroutine(ActivateAndDestroyPlatforms(platformActivationTime,platformDestroyTime));
        }        
    }


    private void OnTriggerEnter(Collider other)
    {
        //Si dentro de la zona de acción esta Saru activo el bool que modela la activacion de plataformas en el bucle principal
        if ((other.gameObject.tag == "Player"))
        {
            runeParticlesEffect.Play();
            isInTrigger = true;
        }
    }

    //Si sale Saru de la zona de acción, desactivo el bool que modela la activacion de plataformas en el bucle principal
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            runeParticlesEffect.Stop();
            isInTrigger = false;
        }
    }



    //Esta corrutina va instanciando secuencialmente la totalidad de plataformas de esta región cada n segundos. Una vez instanciadas todas, comienzo a destruirlas con un intervalo de m segundos entre plataformas
    IEnumerator ActivateAndDestroyPlatforms(float n, float m)
    {
        audioSourceMagic.PlayScheduled(1);
        for (int j = 0; j < numberOfPlatforms; j++)
        {
            yield return new WaitForSeconds(n);
            platformsArray[j].SetActive(true);
            //una vez que ya he activado todas las plataformas, comienzo a destruirlas secuencialmente
            if (j == numberOfPlatforms - 1)
            {
                for (int i = 0; i < numberOfPlatforms; i++)
                {
                    yield return new WaitForSeconds(m);
                    platformsArray[i].SetActive(false);
                }
            }
        }
        
    }

  



}

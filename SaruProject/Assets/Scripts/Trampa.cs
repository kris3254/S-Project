using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

//Clase que modela el comportamiento de las trampas que atrapan a Saru.
//Este script va atachado al prefab de la trampa.
//Mientras Saru esté dentro del área de la trampa, la velocidad de animación de correr/andar se verá ralentizada, a velocidad real únicamente podrá rodar para escapar.
//Estando dentro de la trampa perdera una cantidad de vida por cada N seg, y un efecto en la pantalla (emula dolor) aparecerá en UI. 
//Además, la trampa irá incrementando su tamaño hasta alcanzar un máximo. Nota: Inicializar el tamaño de las plantas a decimales exactos (Ej 0.1 no 0.11

public class Trampa : MonoBehaviour {

    
    public float damageTime;//variable cuyo valor establece cada cuanto tiempo se nos quita vida
    public int damageValue;//modela el numero de unidades de vida que la trampa nos quita cada damageTime dentro de esta.
    
    public Text healthText;// En la version final esto va en el UIManager, tendriamos que coger una referencia a el y que este asignara el valor nuevo de vida a la UI
    private int healthValue;// En la versión final esto va en el PlayerManager, y que este decrementara el valor de vida en el player

    private Animator animatorPlayer;//variable donde se almacena una refencia al componente animator del player
    private AnimatorStateInfo m_CurrentAnimatorStateInfo;
    public float lowerAnimSpeed;// [0,1] velocidad a la que la animacion de correr (y de mov de Saru) se va a ejecutar dentro de la trampa (cuanto menor sea, mayor será la sensación de que estamos atrapados)

    public Texture2D splat;//textura que representa el efecto de daño en pantalla
    private float alpha;//variable de control para mostrar el efecto de daño en la UI, al hacerla distinta de 0 se observa el efecto en pantalla
    public float alphaSpeed;//variable para regular la cantidad de tiempo que el efecto de daño se mantiene en pantalla 

    public float timeBetweenScalingIterations;// parametro para controlar cada cuanto tiempo se produce una iteración de aumento de escala en la corrutina que aumenta el tamaño de la trampa
    public float maxEscaleValue;//tamaño máximo a alcanzar por la trampa (x,z)
    public float incrementScalingFactor;//factor de escala para aumentar el tamaño de la trampa
    private float actualScaleValue;//variable a modo de indice para mostrar el crecimiento progresivo de tamaño de la trampa conform estamos dentro, hasta que alcanza un tamaño maximo variable.


    // Use this for initialization
    void Start () {
        gameObject.GetComponent<MeshRenderer>().enabled = false;// las trampas no se ven, se renderizan cuando las atravieso por primera vez
        healthValue = 90;
        actualScaleValue = gameObject.transform.localScale.x;//La inicializo con el valor actual de x (el de z es el mismo), en y lo mantenemos constante (las trampas son cilindros realmente)
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;// la trampa comienza a verse

            //lanzo la corrutina que va aumentando el tamaño de la trampa hasta un máximo
            StartCoroutine(MakeBigger(timeBetweenScalingIterations, maxEscaleValue, incrementScalingFactor));

            //ralentizo la anim de correr/andar para simular que el personaje está atrapado, tengo que comprobar que entra corriendo o andando, pues si el player es listo y entra directamente rodando no debería de afectarle
            animatorPlayer = other.gameObject.GetComponent<Animator>();
            m_CurrentAnimatorStateInfo = animatorPlayer.GetCurrentAnimatorStateInfo(0);
            if ((m_CurrentAnimatorStateInfo.IsName("Grounded") == true) || (m_CurrentAnimatorStateInfo.IsName("Airborne") == true) || (m_CurrentAnimatorStateInfo.IsName("Crouching")))
             {
                //aqui lo ideal seria que se ejecutara otra animación específica de las trampas) como andando más lento intentando liberarse.
                //other.gameObject.GetComponent<ThirdPersonCharacter>().m_AnimSpeedMultiplier = lowerAnimSpeed;
                //other.gameObject.GetComponent<ThirdPersonCharacter>().m_MoveSpeedMultiplier = lowerAnimSpeed;
             }

            //lanzo la corrutina que empieza a quitar vida la player
            StartCoroutine(DecreaseHealthEveryNSeconds(damageTime));


        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StopAllCoroutines();
  
            //devuelvo la velocidad original al animator del player
          //  other.gameObject.GetComponent<ThirdPersonCharacter>().m_AnimSpeedMultiplier = 1;
           // other.gameObject.GetComponent<ThirdPersonCharacter>().m_MoveSpeedMultiplier = 1;
        }
    }


    //Corrutina para decrementar la vida del player dentro de la trampa
    IEnumerator DecreaseHealthEveryNSeconds(float n)
    {
        while (true)
        {
            alpha = 1f;//en cuanto la variable cambia de valor comienza a ejecutarse el efecto de daño en pantalla
            healthValue -= damageValue;
            healthText.text = healthValue.ToString();
            yield return new WaitForSeconds(n);
    
        }

    }

    //corrutina para aumentar el tamaño de la trampa progresivamente
    IEnumerator MakeBigger (float time, float maxSize, float scaleFactor)
    {
        while (actualScaleValue <= maxSize)
        {
            actualScaleValue += scaleFactor;//cada k seg se incrementa la escala en X y en Y de la trampa 0.1f
            gameObject.transform.localScale = new Vector3(actualScaleValue, 0.24f, actualScaleValue);
            yield return new WaitForSeconds(time);

        }
    }

    //Método que ejecuta el efecto de daño en pantalla, cuando desde el bucle principalponemos el valor de alpha =!0 se ejecuta este metodo, que dibuja la textura en la UI con el valor de alpha que le hemos dado
    // y este va decreciendo frame a frame desapareciendo finalmente, esta velocidad de decrecimiento es configurable a traves de la variable publica alphaSpeed (valores optimos entre 0.6 y 1)
    //Este metodo iría en el UIManager en la versión final  
    void OnGUI()
    {
        if (alpha > 0.0)
        {
            Color color = Color.white;
            color.a = alpha;
            GUI.color = color;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), splat);
            alpha -= alphaSpeed * Time.deltaTime;
        }
    }


}

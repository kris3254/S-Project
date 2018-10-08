using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Para hacer fade in nada mas cargarse la escena arrastrar este script a un objeto de la escena, pasarle la texturam playOnAwake a true y startFadingIn a true y las respectivas duraciones
//Para hacer fadeOut en la escena desde otro script (antes de hacer un SceneManager.LoadScene()) coger objeto.getComponent<SceneFade>().FadeOut(); en la escena que cargo de nuevo habra que hacer un fade in para que el efecto total sea adecuado. 

public class SceneFade : MonoBehaviour {

    public Texture fadeOutTexture;//textura que se mostrara al hacer el fadeOut, por defecto es negro.
    public bool playOnAwake = false;//se va a ejecutar en cuanto se cargue la escena.
    public bool startFadingIn = true;//cuando PlayOnAwake está seleccionado, este check indica si hará fadeIn al empezar, o si por el contrario hará FadeOut.

    public float fadeInDuration = 3;//Duracion efecto fade in
    public float fadeOutDuration = 3;//Duracion efecto fade out

    private bool isFading = false;
    private bool isFadeIn = true;

    private float alpha = 0.0f;
    private bool allowFading = true;

    void Start()
    {
        fadeOutTexture.wrapMode = TextureWrapMode.Repeat;
        isFadeIn = startFadingIn;

        if (isFadeIn)
        {
            alpha = 1.0f;
        }

        isFading = playOnAwake;
    }

    void OnGUI()
    {
        //new alpha value
        if (isFading)
        {
            if (isFadeIn)
            {
                //Debug.Log("fadingin");
                alpha -= Mathf.Clamp01(Time.deltaTime) / (fadeInDuration * 2);
            }
            else
            {
                //Debug.Log("fadingout");
                alpha += Mathf.Clamp01(Time.deltaTime) / (fadeOutDuration * 2);
            }

            if (alpha < 0 || alpha > 1)
            {
                isFading = false; //stop fading
            }
        }
        //draw texture if necessary
        if (allowFading && alpha > 0)
        {
            //Debug.Log(Time.time + " " + alpha + " " + isFading);

            float oldAlpha = GUI.color.a;
            Color c = GUI.color;
            c.a = alpha;
            GUI.color= c;
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture, new Rect(0, 0, 1, 1));
            c.a = oldAlpha;
            GUI.color = c;
        }
    }


    /**
     * Start new Fade In
     * @param {Number} duration 
     */
    public void FadeIn(float duration)
    {
        fadeInDuration = duration;
        alpha = 1.0f;
        FadeInWithoutReset();
    }


    /**
     * Start new Fade Out
     * @param {Number} duration 
     */
    public void FadeOut(float duration)
    {
        fadeOutDuration = duration;
        alpha = 0.0f;
        FadeOutWithoutReset();
    }

    /**
     * Start new Fade In
     */
    public void FadeIn()
    {
        alpha = 1.0f;
        FadeInWithoutReset();
    }

    /**
     * Start new Fade Out
     */
    public void FadeOut()
    {
        alpha = 0.0f;
        FadeOutWithoutReset();
    }

    /**
     * Continue Fade In
     * @param {Number} duration 
     */
    void FadeInWithoutReset()
    {
        allowFading = true;
        isFadeIn = true;
        isFading = true;
    }

    /**
     * Continue Fade Out
     * @param {Number} duration 
     */
    void FadeOutWithoutReset(float duration)
    {
        fadeOutDuration = duration;
        allowFading = true;
        isFadeIn = false;
        isFading = true;
    }

    /**
     * Continue Fade In
     */
    void FadeInWithoutReset(float duration)
    {
        fadeInDuration = duration;
        allowFading = true;
        isFadeIn = true;
        isFading = true;
    }

    /**
     * Continue Fade Out
     */
    void FadeOutWithoutReset()
    {
        allowFading = true;
        isFadeIn = false;
        isFading = true;
    }


    /**
     * Stop the current fading (pause)
     */
    void StopFading()
    {
        isFading = false;
    }

    /**
     * Stop and hide the current fading
     */
    void HideFade()
    {
        allowFading = false;
        isFading = false;
    }

}

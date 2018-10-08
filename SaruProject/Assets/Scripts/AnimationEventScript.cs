using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEventScript : MonoBehaviour {

    public GameObject SceneFadeInOutReference;
    
	public void PlayMainScene()
    {
        SceneFadeInOutReference.GetComponent<SceneFade>().FadeOut();
        Invoke("ChangeScene", 2);//el tiempo que le paso como parametro al invoke ha de ser el mismo que dura el efecto fadeOut en SceneFade.cs FadeOutDuration
    }

    private void ChangeScene()
    {
        AudioManager.instance.StopSound("MusicaMenuPpal");
        AudioManager.instance.PlaySound("SonidoJungla");
        SceneManager.LoadScene("EscenaFinal");
    }
}

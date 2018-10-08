using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;


//Este script modela el comportamiento consistente en ejecutar una animacion grabada con timeline, cuando el player atraviesa el trigger específico de activacion.
public class PlayTimelineClipWhenTrigger : MonoBehaviour {

    public PlayableDirector playableDirector;
    private bool isFirstTime;

    private void Start()
    {
        isFirstTime = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") && (isFirstTime==true))
        {
            playableDirector.Play();
            isFirstTime = false;
        }
    }
  
}

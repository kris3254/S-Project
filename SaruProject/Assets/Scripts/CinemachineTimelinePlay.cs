using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinemachineTimelinePlay : MonoBehaviour {

    public PlayableDirector playableDirector;
 
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player"))
        {
            Debug.Log("Saru Entra Puente");
            playableDirector.Play();
        }

    }

}

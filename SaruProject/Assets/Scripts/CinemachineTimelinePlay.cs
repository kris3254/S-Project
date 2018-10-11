using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinemachineTimelinePlay : MonoBehaviour {

    public PlayableDirector firstPlayableDirector;
    public PlayableDirector secondPlayableDirector;


    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player"))
        {
            Debug.Log("Saru Entra Puente");
            secondPlayableDirector.Stop();
            firstPlayableDirector.Play();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.tag == "Player"))
        {
            Debug.Log("Saru Sale Puente");
            firstPlayableDirector.Stop();
            secondPlayableDirector.Play();
        }
    }

}

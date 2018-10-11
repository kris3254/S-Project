using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinemachineTimelinePlay : MonoBehaviour {

    public PlayableDirector playableDirector;
    private bool isFirstTime;

    private void Start()
    {
        isFirstTime = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") && (isFirstTime == true))
        {
            Debug.Log("Saru Entra Puente");
            playableDirector.enabled = true;
            isFirstTime = false;
        }
    }

}

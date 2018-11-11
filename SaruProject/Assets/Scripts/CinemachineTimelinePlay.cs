using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;

public class CinemachineTimelinePlay : MonoBehaviour {

    public PlayableDirector firstPlayableDirector;
    public PlayableDirector secondPlayableDirector;
    
    public CinemachineBrain cinemachineBrain;

    Vector2 positionCamera;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player"))
        {
            StartCoroutine("EntryAnimation");
            positionCamera = PlayerManager.instance.GetCameraPosition();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.tag == "Player"))
        {
            StartCoroutine("ExitAnimation");
            PlayerManager.instance.SetCameraPosition(positionCamera);
        }
    }

    IEnumerator EntryAnimation()
    {
        yield return new WaitUntil(() => cinemachineBrain.IsBlending == false);
        secondPlayableDirector.Stop();
        firstPlayableDirector.Play();
    }

    IEnumerator ExitAnimation()
    {
        yield return new WaitUntil(() => cinemachineBrain.IsBlending == false);
        firstPlayableDirector.Stop();
        secondPlayableDirector.Play();
    }

}

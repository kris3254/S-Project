using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


//Este script tiene como función reproducir una cinemática previamente grabada en video. Para ello, hace uso de una camara secundaria presente en la escena
//switcheando las prioridades de la principal y esta logramos reproducir la cinematica, y pasar a gameplay.
//Para ello es necesario tener una camara secundaria en la escena con un componente audiosource, un componente videoplayer configurado con RenderMode=>camera Near plane y audiosouce le pasamos
//la de la propia camara (arrastrando este mismo objeto desde el inspector), y play on awake desactivado.
//este script ira en un trigger, que al activarse generara los cambios de camara y reproducira las cinematicas, al finalizar, volvera a modificar sus prioridades para volver a gameplay

//opcionalmente a este script le podemos pasar una posicion de instanciación  para simular la situacion de que al finalizar la cinematica, el player esta en una posicion diferente a la 
//que tenia al triggerearla.

public class PlayCinematic : MonoBehaviour {

    private bool isFirstTime;
    public GameObject cameraForPlayingCinematics;
    private VideoPlayer vp;
    private Camera cinematicCamera;
    public VideoClip cinematicVideoClip;
    public VideoClip startingVideoClip;

    public Camera mainCamera;

    public Transform respawnTransform;
    public playerControllerCustom pController;

    public bool isStartingCinematic = false;

    private void Awake()
    {
        vp = cameraForPlayingCinematics.GetComponent<VideoPlayer>();
        isFirstTime = true;
        cinematicCamera = cameraForPlayingCinematics.GetComponent<Camera>();
    }

    private void Start()
    {
        if (isStartingCinematic == true)
        StartCoroutine("StartingCinematic");
    }


    void EndReached(VideoPlayer videoplayer)
    {
        videoplayer.Stop();

        //justo antes de volver a la camara de gameplay muevo al player a la posicion donde finaliza la cinemática
        //solo en el caso de las cinematicas donde suceda esto (la posicion de respawn es !=null)
        if (respawnTransform != null)
        {
            PlayerManager.instance.gameObject.SetActive(false);
            PlayerManager.instance.gameObject.transform.position = respawnTransform.position;
            PlayerManager.instance.gameObject.SetActive(true);
        }

        UIManager.instance.UnHideHud();
        LevelManager.instance.UnlockPauseMenu();
        cinematicCamera.depth = -1;
        cameraForPlayingCinematics.SetActive(false);
        mainCamera.depth = 0;
        vp.loopPointReached -= EndReached;
        pController.canDoThings = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player") && (isFirstTime))
        {
            vp.loopPointReached += EndReached;
            vp.clip = cinematicVideoClip;
            vp.Play();
            UIManager.instance.HideHud();
            LevelManager.instance.LockPauseMenu();
            mainCamera.depth = -1;
            cinematicCamera.depth = 0;
            cameraForPlayingCinematics.SetActive(true);
            isFirstTime = false;

            pController.canDoThings = false;
            pController.anim.SetFloat("speed", 0f);

        }

    }

    IEnumerator StartingCinematic()
    {
        vp.loopPointReached += EndReached;
        vp.clip = startingVideoClip;
        vp.Play();
        UIManager.instance.HideHud();
        LevelManager.instance.LockPauseMenu();
        mainCamera.depth = -1;
        cinematicCamera.depth = 0;
        cameraForPlayingCinematics.SetActive(true);
        pController.canDoThings = false;
        pController.anim.SetFloat("speed", 0f);
        yield return new WaitForSeconds(0);
    }
    



}

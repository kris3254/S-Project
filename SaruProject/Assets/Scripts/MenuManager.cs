using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour {

    public GameObject saru;

    public GameObject cameraForPlayingCinematics;
    private VideoPlayer vp;
    private Camera cinematicCamera;
    public VideoClip cinematicVideoClip;
    public Camera mainCamera;


    private void Awake()
    {
        vp = cameraForPlayingCinematics.GetComponent<VideoPlayer>();
        cinematicCamera = cameraForPlayingCinematics.GetComponent<Camera>();
        cameraForPlayingCinematics.SetActive(false);
    }


    public void Play()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");
        saru.GetComponent<Animator>().SetBool("isJumping", true);
        AudioManager.instance.PlaySound("IniciarPartida");
    }

    public void Options()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");
    }

    public void Credits()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");
        AudioManager.instance.StopSound("MusicaMenuPpal");
        HideMenu();
        vp.loopPointReached += EndReached;
        vp.clip = cinematicVideoClip;
        vp.Play();
        mainCamera.depth = -1;
        cinematicCamera.depth = 0;
        cameraForPlayingCinematics.SetActive(true);
    }

    public void Exit()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit ();
    #endif

    }
    
    void EndReached(VideoPlayer videoplayer)
    {
        videoplayer.Stop();
        cinematicCamera.depth = -1;
        cameraForPlayingCinematics.SetActive(false);
        UnHideMenu();
        mainCamera.depth = 0;
        AudioManager.instance.PlaySound("MusicaMenuPpal");
        vp.loopPointReached -= EndReached;
    }

    void HideMenu()
    {
        gameObject.SetActive(false);
    }

    void UnHideMenu()
    {
        gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour {

    public static AudioManager instance = null;

    public Sound[] listaSonidos;

    private void Awake()
    {
        //Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in listaSonidos)
        {
            s.audioSource=gameObject.AddComponent<AudioSource>();
            s.audioSource.playOnAwake = false;
            s.audioSource.clip = s.clip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.loop = s.loop;
        }
    }

    //Desde el start reproducimos el sonido de fondo de toda la demo (loop a true en lista sounds)
    public void Start()
    {
        if(SceneManager.GetActiveScene().name=="MainMenu")
        {
            PlaySound("MusicaMenuPpal");
        }
        else if (SceneManager.GetActiveScene().name == "EscenaFinal")
            PlaySound("SonidoJungla");
    }

    public void PlaySound(string name)
    {
       Sound s=Array.Find(listaSonidos, sound => sound.name == name);
       if(s==null)
        {
            Debug.Log("No existe ningun sonido en la lista de sonidos con ese nombre");
            return;
        }
       s.audioSource.Play();
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(listaSonidos, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("No existe ningun sonido en la lista de sonidos con ese nombre");
            return;
        }
        s.audioSource.Stop();
    }
}

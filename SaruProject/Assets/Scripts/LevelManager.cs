using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Este script lo tiene atachado un gameobject vacio en la escena

//El tema de Respawnear funciona de la siguiente manera:
//Habrá diseminadas por el escenario diversos chekpoints con un collider asociado (con transforms que seran los puntos de Spawn)
//Los Checkpoints tendran un script "SpawnPoint", en cuyo metodo OnTriggerEnter() actualizo el valor de la variable lastSpawnPoint (perteneciente a este script) llamando al metodo UpdateSpawnPoint()
//al respawnear el player vuelve a tener los 3 puntos de vida.

public class LevelManager : MonoBehaviour {

    public static LevelManager instance = null;


    private int numMiniOrbesLevel;// numero máximo de miniorbes repartidos por el nivel
    private int numMiniOrbesPlayer;// numero de miniOrbes que el player ha recogido hasta el momento

    public float respawnDelay;//retardo que se produce antes de ejecutarse el metodo de respawn del player (tener en cuenta la duracion del efecto FadeInOut al morir), con 2s de respawnDelay queda perfectamente secuenciado con el FadeInOut
    public Transform initialRespawnPosition;//variable representa la posicion de respawn por defecto (por si el player muere antes de pasar por un respawn point)
    private Transform lastPlayerSpawnPoint;//transform correspondiente al SpawnPoint que el player atraviesa

    private bool isPaused;

    private bool flagPauseMenu;//variable de control para bloquear/desbloquear el que se pueda apretar el menu de pausa

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        isPaused = false;
        numMiniOrbesLevel = 0;
        numMiniOrbesPlayer = 0;
        UpdateSpawnPoint(initialRespawnPosition);//inicializamos con una posicion inicial de respawn por defecto
        flagPauseMenu = true;
    }


    private void Update()
    {
        //El update tiene que estar checkeando si el jugador presiona o no el boton de pausa (P)
        if (((Input.GetKeyDown(KeyCode.Escape)) || (Input.GetButtonDown("Start_PS4"))) && flagPauseMenu)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        

    }

    //Metodo para bloquear el menu Pausa
    public void LockPauseMenu()
    {
        flagPauseMenu = false;
    }

    //Metodo para desbloquear el menu de pausa 
    public void UnlockPauseMenu()
    {
        flagPauseMenu = true;
    }
 
    //Metodo que se ejecuta al morir el player, respawnea en la posicion correspondiente al ultimo chekpoint que ha atravesado
    public void Respawn()
    {
        StartCoroutine(RespawnCorroutine(respawnDelay)); 
    }


    //Metodo al que llamamos desde el spawnPoint, para actualizar como posicion de respawneo el último punto de respawn atravesado por el jugador
    public void UpdateSpawnPoint(Transform playerSpawnPoint)
    {
        lastPlayerSpawnPoint = playerSpawnPoint;
    }

    
    IEnumerator RespawnCorroutine(float f)
    {
        PlayerManager.instance.gameObject.SetActive(false);
        yield return new WaitForSeconds(f);
        PlayerManager.instance.gameObject.transform.position = lastPlayerSpawnPoint.position;//modifico su posicion y le asigno la del ultimo SpawnPoint atravesado
        PlayerManager.instance.gameObject.SetActive(true);
        PlayerManager.instance.SetHealth(3);// reseteo su vida a 3
    }

    //Metodo que pone en pausa el juego y muestra el panel de pausa o muerte en funcion de la vida del player
    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;        
        UIManager.instance.ShowPausePanel();
    }

    //Metodo que resume el juego 
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;        
        UIManager.instance.UnshowPausePanel();
    }

    public void AddMiniOrbToTotalCount()
    {
        numMiniOrbesLevel++;
        UIManager.instance.UpdateNumMiniOrbsLevelText(numMiniOrbesLevel);
    }

    public void AddMiniOrbToPlayerCount()
    {
        numMiniOrbesPlayer++;
        UIManager.instance.UpdateNumMinirOrbsPlayerText(numMiniOrbesPlayer);
    }

    
	
}

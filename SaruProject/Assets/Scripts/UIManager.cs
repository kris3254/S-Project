using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdvancedUtilities.Cameras;
using UnityEngine.EventSystems;



//Este script lo tiene atachado el canvas de la escena
public class UIManager : MonoBehaviour {

    public static UIManager instance = null;

    public Image maskImage;//Imagen de la mascara de Saru
    public Image life1;//imagen de uno de los elementos de vida del player
    public Image life2;//imagen de uno de los elementos de vida del player
    public Image life3;//imagen de uno de los elementos de vida del player

    public GameObject pausePanel;//panel del menu de pausa con los botones continue, continue from last checkpoint, restart game y collectables
    public GameObject continuePauseMenuButton;
    public GameObject exitPauseMenuButton;
    public GameObject restartPauseMenuButton;
    public GameObject collectablesPauseMenuButton;

    public GameObject ConceptsVisualizationGameObject;//GameObject que contiene el panel de visualizacion de concepts, y los tres botones para navegar por estos concepts.
    public GameObject previousConceptButton;
    public GameObject nextConceptButton;
    public GameObject backToPauseButton;
    private int conceptListIndex;



    public GameObject fadeInOutPanel;
    public float fadeInOutDelayValue;
    public float fadeInOutValue;//valor tanto de alpha como de tiempo que se produce para generar el efecto FadeInOut (valor de incremento/decremento del alpha y de tiempo entre incrementos/decrementos)

    public GameObject controllerCamera;//objeto que contiene el script BasicCameraController (CameraController dentro del objeto CameraAndPlayer)
    public float ShakeEffectDuration;//tiempo que el efecto de camara se mantiene activo

    public GameObject conceptPanel;//Panel que albergará como imágen de fondo la del concept asocidado a un coleccionable que mostramos al ser recogido por el player.
    public float conceptInScreenTime;// valor de tiempo que el concept asociado al coleccionable se muestra en pantalla.

    public GameObject tutorialPanel;//panel que contiene todos los objetos de UI relacionados con el tutorial
    public float tutorialMessageOnScreenTime; //variable que contiene el tiempo que el mensaje tutorial va a permanecer en pantalla.

    public Image collecionableImage;
    public Text numMiniOrbsLevelText;
    public Text numMiniOrbsPlayerText;
    public Text separadorNumOrbes;

	//Dialogos
	Queue<string> textsToShow = new Queue<string>();
	[SerializeField]
	Text dialogText;
	public GameObject dialogPanel;
	public float timeBetweenDialogTexts;
	bool canEnter = true;
	public float cdForInput = 0.2f;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

       //DontDestroyOnLoad(gameObject);
        InitializeHud();
    }

	void Update(){
		if (dialogPanel.activeSelf && Input.GetAxisRaw ("X_PS4")==1 && canEnter) {
			canEnter = false;
			Invoke ("CanEnterNow",cdForInput);
			ChangeText ();
		}
		if(conceptPanel.activeSelf && Input.GetAxisRaw("O_PS4")==1){
			conceptPanel.SetActive(false);
		}
	}

	void CanEnterNow(){
		canEnter = true;
	}

    //Metodo que inicializa todos los elementos del HUD, activando los deseados y desactivando los temporales
    public void InitializeHud()
    {
        maskImage.gameObject.SetActive(true);
        life1.gameObject.SetActive(true);
        life2.gameObject.SetActive(true);
        life3.gameObject.SetActive(true);

        pausePanel.gameObject.SetActive(false);
        continuePauseMenuButton.SetActive(true);
        exitPauseMenuButton.SetActive(true);
        restartPauseMenuButton.SetActive(true);
        collectablesPauseMenuButton.SetActive(true);

        ConceptsVisualizationGameObject.SetActive(false);
        previousConceptButton.gameObject.SetActive(true);
        nextConceptButton.gameObject.SetActive(true);
        backToPauseButton.gameObject.SetActive(true);
        conceptListIndex = 0;
        

        fadeInOutPanel.gameObject.SetActive(false);

        tutorialPanel.gameObject.SetActive(false);

        dialogPanel.gameObject.SetActive(false);

        collecionableImage.gameObject.SetActive(true);
        numMiniOrbsLevelText.text = "0";
        numMiniOrbsLevelText.gameObject.SetActive(true);
        numMiniOrbsPlayerText.text = "0";
        numMiniOrbsPlayerText.gameObject.SetActive(true);
        separadorNumOrbes.gameObject.SetActive(true);

    }

    //Metodo que se ejecuta siempre que el valor de vida del player varia, refleja este cambio en la UI
    //las vidas se van eliminando de derecha a izquierda en el HUD
    public void UpdateHealthHUD()
    {
        int life = PlayerManager.instance.GetHealthValue();
        switch (life)
        {
            case 0:
                life1.gameObject.SetActive(false);
                life2.gameObject.SetActive(false);
                life3.gameObject.SetActive(false);
                break;

            case 1:
                life1.gameObject.SetActive(true);
                life2.gameObject.SetActive(false);
                life3.gameObject.SetActive(false);
                break;

            case 2:
                life1.gameObject.SetActive(true);
                life2.gameObject.SetActive(true);
                life3.gameObject.SetActive(false);
                break;

            case 3:
                life1.gameObject.SetActive(true);
                life2.gameObject.SetActive(true);
                life3.gameObject.SetActive(true);
                break;

        }
    }


    public void ShowPausePanel()
    {
        AudioManager.instance.PlaySound("AbrirMenuPausa");
        pausePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(continuePauseMenuButton);

    }

    public void UnshowPausePanel()
    {
        AudioManager.instance.PlaySound("CerrarMenuPausa");
        pausePanel.SetActive(false);
    }

    //Metodo que se ejecuta al presionar el boton de Continue del deathAndPausePanel
    public void ExitGameButtonPressed()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit ();
        #endif
    }

    //Metodo que se ejecuta al presionar el boton de Restart del deathAndPausePanel
    public void RestartGameButtonPressed()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");
        LevelManager.instance.ResumeGame();
        LevelManager.instance.UpdateSpawnPoint(LevelManager.instance.initialRespawnPosition);//establecemos como posicion de respawn la de inicializacion
        LevelManager.instance.Respawn();//respawneamos al player en esta posicion
        Debug.Log("Restart Game Pressed");
    }

    public void ContinueGameButtonPressed()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");
        LevelManager.instance.ResumeGame();
        Debug.Log("Continue game Pressed");
    }

    public void CollectablesButtonPressed()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");
        Debug.Log("Collectables Button Pressed");

        if (PlayerManager.instance.collectablesList.Count > 0)// si la lista de concepts descubiertos por el player no esta vacia
            conceptPanel.GetComponent<Image>().sprite = PlayerManager.instance.collectablesList[conceptListIndex];

        ConceptsVisualizationGameObject.SetActive(true);
        conceptPanel.SetActive(true);
        pausePanel.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(previousConceptButton);
        LevelManager.instance.LockPauseMenu();

    }

    public void NextConceptButtonPressed()
    {
        AudioManager.instance.PlaySound("MostrarMensajeTutorial");
        conceptListIndex++;

        if (conceptListIndex >= PlayerManager.instance.collectablesList.Count)
        {
            Debug.Log("valor de indice de la lista de concepts no valido");
            conceptListIndex--;
        }

        conceptPanel.GetComponent<Image>().sprite = PlayerManager.instance.collectablesList[conceptListIndex];

    }

    public void PreviousConceptButtonPressed()
    {
        AudioManager.instance.PlaySound("MostrarMensajeTutorial");
        conceptListIndex--;

        if (conceptListIndex < 0)// el indice nunca pude ser menor que 0, 0 corresponde al primer elemento de la lista
        {
            conceptListIndex++;
        }

        conceptPanel.GetComponent<Image>().sprite = PlayerManager.instance.collectablesList[conceptListIndex];

    }

    public void BackToPauseMenuButtonPressed()
    {
        AudioManager.instance.PlaySound("ElegirOpcionMenu");
        ConceptsVisualizationGameObject.SetActive(false);
        conceptPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(continuePauseMenuButton);
        LevelManager.instance.UnlockPauseMenu();
    }



    public void FadeInOutEffect()
    {
        //StopAllCoroutines();//Para que no se ejecuten a la vez la corrutina del efecto de daño y la de la muerte
        StartCoroutine(FadeInOutCorroutine(fadeInOutDelayValue, fadeInOutValue));
    }

    IEnumerator FadeInOutCorroutine(float delay, float time)
    {
        fadeInOutPanel.gameObject.SetActive(true);
        Image imagen = fadeInOutPanel.GetComponent<Image>();

        AudioManager.instance.PlaySound("GritoMuerte");

        for (float i = 0; i <= 1f + time; i += time)
        {
            Color c = imagen.color;
            c.a = i;
            imagen.color = c;
            yield return new WaitForSeconds(time);
        }

        yield return new WaitForSeconds(delay);

        for (float i = 1f; i >= -time; i -= time)
        {
            Color c = imagen.color;
            c.a = i;
            imagen.color = c;
            yield return new WaitForSeconds(time);
        }
        AudioManager.instance.StopSound("GritoMuerte");

        fadeInOutPanel.gameObject.SetActive(false);
    }

    public void CameraShake()
    {
        StartCoroutine(CameraShakeCorroutine(ShakeEffectDuration));
    }

    //Corrutina que genera el efecto de cameraShake, los parametros del efecto de camera como la intensidad, los ejes a los que afecta etc se configura desde el inspector en el objeto CameraAndPlayer=>CameraController=> ScreenShake
    IEnumerator CameraShakeCorroutine(float shakeEffectTime)
    {
        controllerCamera.GetComponent<BasicCameraController>().ScreenShake.Enabled = true;
        yield return new WaitForSeconds(shakeEffectTime);
        controllerCamera.GetComponent<BasicCameraController>().ScreenShake.Enabled = false;
    }

    public void ShowConcept(Sprite concept)
    {
        StartCoroutine(ShowConceptCorroutine(conceptInScreenTime, concept));
    }

    IEnumerator ShowConceptCorroutine(float time, Sprite concept)
    {
        conceptPanel.GetComponent<Image>().sprite = concept;//asociamos como imagen de fondo del panel base de los concepts, el concept asociado al coleccionable en concreto
        conceptPanel.SetActive(true);
        AudioManager.instance.PlaySound("RecogerColeccionable");
        yield return new WaitForSeconds(time);
        conceptPanel.SetActive(false);
    }

    //Este metodo llama a una corrutina que va mostrando los distintos mensajes (gameobjects con textos y/o imagenes) cada timeBetweenDialogTexts.
    //todos los mensajes (desactivados) a mostrar que pertenezcan a un mismo tutorial tienen que estar contenidos en un gameObject padre (activo) dentro del panel TutorialPanel
    public void ShowTutorialMessage(GameObject[] tutorialMessages)
    {
        StopAllCoroutines();
        foreach (GameObject g in tutorialMessages)
        {
            if (g != null)
            {
                g.SetActive(false);
            }
        }
        StartCoroutine(ShowTutorialMessageCorroutine(tutorialMessageOnScreenTime, tutorialMessages));
    }

    IEnumerator ShowTutorialMessageCorroutine(float time, GameObject[] tutorialMessages)
    {
        tutorialPanel.SetActive(true);
        foreach (GameObject g in tutorialMessages)
        {
            if (g != null)
            {
                Debug.Log("Mostramos texto tutorial");
                g.SetActive(true);
                AudioManager.instance.PlaySound("MostrarMensajeTutorial");
                yield return new WaitForSeconds(time);
                g.SetActive(false);
            }
        }
        tutorialPanel.SetActive(false);
    }

    //Este metodo llama a una corrutina que va mostrando los distintos mensajes (gameobjects con textos y/o imagenes) cada timeBetweenDialogTexts.
    //todos los mensajes (desactivados) a mostrar que pertenezcan a un mismo dialogo tienen que estar contenidos en un gameObject padre (activo) dentro del panel DialogPanel
    /*
    public void ShowDialog(GameObject[] texts)
    {
        StopAllCoroutines();
        foreach (GameObject g in texts)
        {
            if (g != null)
            {
                g.SetActive(false);
            }
        }
        StartCoroutine(ShowDialogCorroutine(texts, timeBetweenDialogTexts));
    }

    IEnumerator ShowDialogCorroutine(GameObject[] texts, float time)
    {
        bool flag = false;
        dialogPanel.SetActive(true);
        foreach (GameObject g in texts)
        {
            if (g != null)
            {
                Debug.Log("Mostramos texto dialogo");
                g.SetActive(true);

                if (flag)
                {
                    AudioManager.instance.PlaySound("CiudadanoHablando1");
                }
                else
                    AudioManager.instance.PlaySound("CiudadanoHablando2");
                flag = !flag;

                yield return new WaitForSeconds(time);
                g.SetActive(false);
            }
        }
        dialogPanel.SetActive(false);
    }
	*/

    public void UpdateNumMiniOrbsLevelText(int numMiniOrbsLevel)
    {
        numMiniOrbsLevelText.text = numMiniOrbsLevel.ToString();
    }

    public void UpdateNumMinirOrbsPlayerText(int numMiniOrbsPlayer)
    {
        numMiniOrbsPlayerText.text = numMiniOrbsPlayer.ToString();
    }

    //metodo que se llama al ejecutarse una cinematica, para mostrar/ocultar las vidas y coleccionables del HUD
    public void HideHud()
    {
        life1.gameObject.SetActive(false);
        life2.gameObject.SetActive(false);
        life3.gameObject.SetActive(false);
        maskImage.gameObject.SetActive(false);

        collecionableImage.gameObject.SetActive(false);
        numMiniOrbsLevelText.gameObject.SetActive(false);
        numMiniOrbsPlayerText.gameObject.SetActive(false);
        separadorNumOrbes.gameObject.SetActive(false);
    }

    //metodo que se llama al ejecutarse una cinematica, para mostrar/ocultar las vidas y coleccionables del HUD
    public void UnHideHud()
    {
        life1.gameObject.SetActive(true);
        life2.gameObject.SetActive(true);
        life3.gameObject.SetActive(true);
        maskImage.gameObject.SetActive(true);

        collecionableImage.gameObject.SetActive(true);
        numMiniOrbsLevelText.gameObject.SetActive(true);
        numMiniOrbsPlayerText.gameObject.SetActive(true);
        separadorNumOrbes.gameObject.SetActive(true);
    }

	public void AddDialogText(string text){
		textsToShow.Enqueue (text);
		if (!dialogPanel.activeSelf) {
			dialogPanel.SetActive (true);
			dialogText.text = text;
			Invoke ("ChangeText", timeBetweenDialogTexts);
		}
			
	}

	void ChangeText(){
		textsToShow.Dequeue ();
		CancelInvoke ("ChangeText");
		if (textsToShow.Count <= 0)
			dialogPanel.SetActive (false);
		else {
			dialogText.text = textsToShow.Peek ();
			Invoke ("ChangeText", timeBetweenDialogTexts);
		}
	}



}

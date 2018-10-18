using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class DemoEnd : MonoBehaviour {

    public GameObject fadePanel;
    public GameObject fadeImage;
    public playerControllerCustom pController;
    public PlayableDirector playableDirector;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartFade() 
    {
        
            fadePanel.SetActive(true);
            fadeImage.SetActive(true);
            fadePanel.GetComponent<CanvasGroup>().alpha = 0;
            fadeImage.GetComponent<CanvasGroup>().alpha = 0;
            StartCoroutine(DemoFade(0f, 1f));
                
    }

    IEnumerator DemoFade(float initialAlpha, float finalAlpha)
    {
        bool continueLoop = true;
        yield return new WaitForSeconds(0.5f);
        pController.canDoThings = false;
        AudioManager.instance.PlaySound("MusicaPoblado");
        playableDirector.Play();
        yield return new WaitForSeconds(12f);
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / 3f;
        while (continueLoop)
        {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / 3f;
            if (percentageComplete >= 1)
            {
                continueLoop = false;
                percentageComplete = 1;
            }
            fadePanel.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(initialAlpha, finalAlpha, percentageComplete);
            fadeImage.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(initialAlpha, finalAlpha, percentageComplete);
            yield return new WaitForEndOfFrame();
            if (continueLoop == false)
            {
                yield return new WaitForSeconds(5f);
                fadePanel.SetActive(false);
                fadeImage.SetActive(false);
                AudioManager.instance.StopSound("MusicaPoblado");
                AudioManager.instance.PlaySound("MusicaMenuPpal");

                SceneManager.LoadScene("MainMenu");
            }
        }

    }
}

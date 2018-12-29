using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenInteractable : MonoBehaviour {

    public GameObject triangleInteractable;
	public string[] dialogTexts;
    public playerControllerCustom pController;
    protected bool isOnTrigger = false;
	protected bool canSendDialog = true;


    virtual protected void Update()
    {
        if (isOnTrigger && ((Input.GetKeyDown(KeyCode.T)) || (Input.GetButtonDown("Triangle_PS4"))) )
        {
			if (!canSendDialog)
				return;
            pController.anim.SetFloat("speed", 0f);
            pController.canDoThings = false;
            Debug.Log("El player ha entrado en el area del ciudadano y ha pulsado la tecla de dialogo");
			for (int i = 0; i < dialogTexts.Length; i++) {
				if(i!=dialogTexts.Length-1)
					UIManager.instance.AddDialogText (dialogTexts[i],this, "DoWhenStartText", null);
				else
					UIManager.instance.AddDialogText (dialogTexts[i],this, "DoWhenStartText", "DoWhenEndText");
			}
			canSendDialog = false;
            //UIManager.instance.ShowDialog(dialogTexts);
        }
    }

	private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            Debug.Log("El player ha entrado en el area del ciudadano");
            pController.modoGuardianActivado = true;
            triangleInteractable.gameObject.SetActive(true);
            isOnTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("El player ha salido del area del ciudadano");
            triangleInteractable.gameObject.SetActive(false);
            isOnTrigger = false;
        }
    }

	void DoWhenStartText(){
		int n = Random.Range(0, 2);
		switch (n)
		{
		case 0:
			AudioManager.instance.PlaySound("CiudadanoHablando1");
			break;
		case 1:
			AudioManager.instance.PlaySound("CiudadanoHablando2");
			break;
		}
	}

	virtual protected void DoWhenEndText(){
		canSendDialog = true;
	}
}

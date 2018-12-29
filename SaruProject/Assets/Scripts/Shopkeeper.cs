using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : CitizenInteractable {

    public GameObject shopCanvas;
    private bool isShopActive = false;
	// Update is called once per frame
	override protected void DoWhenEndText() {
        ActivateShop(true);
	}

    override protected void Update()
    {
        base.Update();
        if( isOnTrigger && Input.GetButtonDown("O_PS4") && isShopActive)
            ActivateShop(false);
    }

    void ActivateShop(bool isActive)
    {
        shopCanvas.SetActive(isActive);
        isShopActive = isActive;
        pController.canDoThings = !isActive;
        canSendDialog = !isActive;
    }

}

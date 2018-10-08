using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDamage : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            int health = PlayerManager.instance.GetHealthValue();
            PlayerManager.instance.DecrementHealth(health);
        }
    }
}

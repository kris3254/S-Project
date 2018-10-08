using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemyCollision : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Enemy")
        {
            other.gameObject.GetComponent<BallEnemyController>().Explode();
            other.gameObject.GetComponent<BallEnemyController>().Die();
        }
    }
}

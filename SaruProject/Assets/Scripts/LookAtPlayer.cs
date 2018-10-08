using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {

    public GameObject player;
	
	// Update is called once per frame
	void Update () {
        var dir = transform.position - player.transform.position;
        dir.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}

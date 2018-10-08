using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestruirRamen : MonoBehaviour {

    public GameObject ramen;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(ramen);
        }
    }
}

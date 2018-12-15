using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyMadera : MonoBehaviour {

    public GameObject destroyedVersion;

    private Component[] trocitosRB;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameObject newMadera = Instantiate(destroyedVersion, transform.position, transform.rotation) as GameObject;
            Destroy(GetComponent<Transform>().GetChild(0).gameObject);

            trocitosRB = newMadera.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in trocitosRB)
                rb.AddForce(Random.Range(0,3), Random.Range(0,3), Random.Range(0,3), ForceMode.Impulse);
            // newMadera.GetComponentInChildren<Rigidbody>().AddForce(0, 3, 3, ForceMode.Impulse);

            Destroy(newMadera, 5f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyMadera : MonoBehaviour {

    public GameObject[] destroyedVersion;
    public GameObject oldMadera;

    private Component[] trocitosRB;
    private Component[] trocitosCollider;
    private bool _maderaDestroyed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameObject newMadera = Instantiate(destroyedVersion[Random.Range(0,2)], new Vector3(transform.position.x, transform.Find("Bone001").transform.Find("Bone002").position.y - 1.5f, transform.position.z), transform.rotation) as GameObject;
            // He probado con la position de todos los hijos de Woodball Enemy, la localPosition, buscando la diferencia de alturas... y no hay manera :>!

            trocitosCollider = newMadera.GetComponentsInChildren<Collider>();

            foreach (Collider collider in trocitosCollider)
                Physics.IgnoreCollision(collider, GetComponent<Collider>());

            if (!_maderaDestroyed)
            {
                Destroy(oldMadera);
                _maderaDestroyed = true;
            }

            trocitosRB = newMadera.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in trocitosRB)
                rb.AddForce(Random.Range(0,3), Random.Range(0,3), Random.Range(0,3), ForceMode.Impulse);

            Destroy(newMadera, 5f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyMadera : BallEnemyController
{

    public GameObject[] destroyedVersion;
    public GameObject oldMadera;
    public float damageDelay;

    private Component[] trocitosRB;
    private Component[] trocitosCollider;
    private bool _maderaDestroyed = false;
    private bool _canTakeDamageAgain = false;

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.O))
        {
            GameObject newMadera = Instantiate(destroyedVersion[Random.Range(0,2)], new Vector3(transform.position.x, transform.Find("Bone001").transform.Find("Bone002").position.y - 1.5f, transform.position.z), transform.rotation) as GameObject;
            // He probado con la position de todos los hijos de Woodball Enemy, la localPosition, buscando la diferencia de alturas... y no hay manera :>!

            trocitosCollider = newMadera.GetComponentsInChildren<Collider>();

            foreach (Collider collider in trocitosCollider)
                Physics.IgnoreCollision(collider, GetComponent<Collider>());

            if (!_maderaDestroyed)
            {
                oldMadera.SetActive(false);
                _maderaDestroyed = true;
            }

            trocitosRB = newMadera.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in trocitosRB)
                rb.AddForce(Random.Range(0,3), Random.Range(0,3), Random.Range(0,3), ForceMode.Impulse);

            Destroy(newMadera, 5f);
        }
    }

    public override void NotInRange()
    {
        base.NotInRange();
    }

    public override void Explode()
    {
        if (_maderaDestroyed && _canTakeDamageAgain)
            Debug.Log ("Entro al override");

        else
        {
            Debug.Log("Entro y no exploto");
            StartCoroutine("WaitCorroutine");
            PlayerManager.instance.DecrementHealth(1);
            this.gameObject.GetComponent<Rigidbody>().AddForce(0, 0, this.transform.position.z * -5);
        }
    }

    public override void Die()
    {
        base.Die();
    }

    public override void Respawn()
    {
        base.Respawn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireBall" && !_maderaDestroyed || other.tag == "Player" && !_maderaDestroyed)
        {
            GameObject newMadera = Instantiate(destroyedVersion[Random.Range(0, 2)], new Vector3(transform.position.x, transform.Find("Bone001").transform.Find("Bone002").position.y - 1.5f, transform.position.z), transform.rotation) as GameObject;

            trocitosCollider = newMadera.GetComponentsInChildren<Collider>();

            foreach (Collider collider in trocitosCollider)
                Physics.IgnoreCollision(collider, GetComponent<Collider>());


            oldMadera.SetActive(false);
            _maderaDestroyed = true;


            trocitosRB = newMadera.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in trocitosRB)
                rb.AddForce(Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3), ForceMode.Impulse);

            Destroy(newMadera, 5f);
        }
    }

    IEnumerator WaitCorroutine()
    {
        yield return new WaitForSeconds(damageDelay);
        _canTakeDamageAgain = true;

    }

}

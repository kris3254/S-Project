//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class BallEnemyController : MonoBehaviour
//{

//    public Transform saru;
//    public Transform baston;
//    public Transform ballCenter;

//    public GameObject SaruGameObject;
//    public GameObject ballExplosion;
//    public GameObject ballDeath;

//    private NavMeshAgent agent;
//    private float distanceToSaru;
//    private float distanceToBaston;

//    private Vector3 initialPosition;
//    private bool _isDeath = false;

//    void Start()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        initialPosition = transform.position;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        distanceToSaru = (transform.position - saru.position).magnitude;
//        distanceToBaston = (transform.position - baston.position).magnitude;

//        if (distanceToSaru < 15f)
//        {
//            agent.SetDestination(saru.position);
//            if ( (distanceToSaru < 0.5f) && (saru.GetComponent<playerControllerCustom>().isAttacking==true) )
//            {
//                //estas cerca de saru, atacale
//                if (!_isDeath)
//                    Die();
//            }
//            if ((distanceToSaru < 0.5f) && (saru.GetComponent<playerControllerCustom>().isAttacking == false))
//            {
//                //estas cerca de saru, atacale
//                if (!_isDeath)
//                {
//                    Explode();
//                    Die();
//                }
//            }
//        }
//        if (distanceToSaru >= 15f)
//        {
//            //si estas lejos de saru vuelve a tu posicion inicial
//            agent.SetDestination(initialPosition);
//        }
//        //falta, si tocas el baston de saru Die()

//    }

//    public void Explode()
//    {
//        Instantiate(ballExplosion, ballCenter.position, Quaternion.identity);

//        _isDeath = true;
//        PlayerManager.instance.DecrementHealth(1);

//        //particulas explosion
//        Destroy(this.gameObject, 0.1f);
//    }
//    public void Die()
//    {
//        Instantiate(ballExplosion, ballCenter.position, Quaternion.identity);
//        _isDeath = true;

//        //particulas muerte
//        //ballDeath.Play();
//        Destroy(this.gameObject, 0.1f);
//    }
//}

    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BallEnemyController : EnemyBase
{

    public Transform saru;
    public Transform baston;
    public Transform ballCenter;
    public GameObject ballExplosion;
    public GameObject ballDeath;

    private NavMeshAgent agent;
    private float distanceToSaru;
    private float distanceToBaston;

    private Vector3 initialPosition;
    private bool _isDeath = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
        LevelManager.instance.RespawnEnemies += Respawn;
    }

    override public void TakeDamage(int i)
    {
        if (_isDeath)
            return;
        Instantiate(ballDeath, ballCenter.position, Quaternion.identity);
        Die();
    }
    // Update is called once per frame
    void Update()
    {
        if (ballCenter.position.y < 14)
        {
            Debug.Log(ballCenter.position.y);
        }
        if (_isDeath) return;
        distanceToSaru = (transform.position - saru.position).magnitude;

        if (distanceToSaru < 15f)
        {
            agent.SetDestination(saru.position);
            if (distanceToSaru < 0.7f)
            {
                //estas cerca de saru, atacale
                if (!_isDeath)
                    Explode();
            }
        }
        if (distanceToSaru >= 15f)
        {
            //si estas lejos de saru vuelve a tu posicion inicial
            agent.SetDestination(initialPosition);
        }
        //falta, si tocas el baston de saru Die()

    }

    public void Explode()
    {
        
        PlayerManager.instance.DecrementHealth(1);
        Instantiate(ballExplosion, ballCenter.position, Quaternion.identity);

        //particulas explosion
        Die();
    }
    public void Die()
    {    
        _isDeath = true;
        //particulas muerte
        //ballDeath.Play();
        gameObject.SetActive(false);
        transform.position = initialPosition;
    }

    public void Respawn()
    {
        _isDeath = false;
        gameObject.SetActive(true); 
    }
}

   
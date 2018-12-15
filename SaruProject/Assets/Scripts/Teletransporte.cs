using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teletransporte : MonoBehaviour {
    [SerializeField]
    public Transform[] positionSpawn;
    public GameObject player;
	// Use this for initialization
	void Start () {
        positionSpawn = new Transform[transform.childCount];

        for (int i = 0; i < positionSpawn.Length; i++)
        {
            positionSpawn[i] = transform.GetChild(i).GetChild(1).transform;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MovePlayer(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MovePlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MovePlayer(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MovePlayer(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            MovePlayer(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            MovePlayer(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            MovePlayer(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            MovePlayer(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            MovePlayer(8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            MovePlayer(9);
        }

    }

    void MovePlayer(int  position)
    {
        if(position >= positionSpawn.Length)  
                return; 
        player.transform.position = positionSpawn[position].position;
    }
}

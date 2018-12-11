using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AudiolistenerRotate : MonoBehaviour {

    public CinemachineBrain cmbrain;

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cmbrain.ActiveVirtualCamera.VirtualCameraGameObject.transform.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformasModoGuardian : MonoBehaviour {

    public playerControllerCustom saruController;

    private MeshCollider _mCollider;
    private BoxCollider _bCollider;
    private MeshRenderer _mRenderer;

	// Use this for initialization
	void Start () {
        _mCollider = (GetComponent<MeshCollider>() != null) ? GetComponent<MeshCollider>() : null;
        _bCollider = (GetComponent<BoxCollider>() != null) ? GetComponent<BoxCollider>(): null;
        _mRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_bCollider != null)
        {
            _bCollider.enabled = saruController.modoGuardian;
        }
        if (_mCollider != null)
        {
            _mCollider.enabled = saruController.modoGuardian;
        }
        _mRenderer.enabled = saruController.modoGuardian;
	}
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControllerCustom : MonoBehaviour
{
    #region variables   
    public float jumpForce = 15f;
    public float moveSpeed = 10f;
	private float maxSpeed;
    public float gravityScale = 4f;
    public float rotateSpeed = 5f;
    public float rollSpeed = 20;

    public float distanceToGround;

    public Transform pivot;
    public GameObject playerModel;
    public Animator anim;
    public Camera cam;
    public bool modoGuardian = false;

    public Texture[] textures;
    public GameObject playerMesh;

    private CharacterController _characterController;
    private Vector3 _moveDirection;
    private float _actSpeed;
    public bool isRolling;

    [HideInInspector]
    public bool isAttacking;
	private bool attackCombo1 = false;
	private bool attackCombo2 = false;
	private bool attackCombo2Time = true;
	public bool canAttack = false;
	public BoxCollider cayadoCollider;

    private int _lerpSpeed = 1;
    private Color _colorModoSaru = new Color(1, 1, 1, 1);
    private Color _colorModoGuardian = new Color(1, 0, 1, 1);
    private float _lerpTime = 0;
    private bool _isGrounded = true;
    private bool _lastFrameGrounded;

    private CameraFilterPack_Color_RGB _rgbColorFilter;
    private CameraFilterPack_3D_Anomaly _anomalyFilter;

    private bool _cambiandoModo = false;

    #endregion

    void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _rgbColorFilter = cam.GetComponent<CameraFilterPack_Color_RGB>();
        _anomalyFilter = cam.GetComponent<CameraFilterPack_3D_Anomaly>();

        _rgbColorFilter.ColorRGB = _colorModoSaru;
        _anomalyFilter.Intensity = 0;
		maxSpeed = moveSpeed;
	}

    void Update()
    {
        HandleIsGrounded();
        HandleGroundedMovement();
        HandleAirMovement();
        HandlePlayerRotation();
		HandleAttacking();
        HandleAnimations();
        HandleGuardianMode();
    }

    private void FixedUpdate()
    {
        
    }
    void HandleIsGrounded()
    {
        RaycastHit hit;
        _lastFrameGrounded = _characterController.isGrounded;
        _isGrounded = _characterController.isGrounded;
        if(Physics.Raycast(transform.position,Vector3.down,out hit ,distanceToGround))
        {
            Debug.Log(hit.transform.name);
            _isGrounded = true;
        }

        if (_characterController.isGrounded == true && _lastFrameGrounded == false)
        {
            _moveDirection.y = 0;
        }

    }
    void HandlePlayerRotation()
    {
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !isRolling)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0f, _moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }
    }
    void HandleAnimations()
    {
        anim.SetBool("IsRolling", isRolling);
        anim.SetBool("IsGrounded", _isGrounded);

		anim.SetBool ("AttackCombo2",attackCombo2);
		if (attackCombo2 && anim.GetCurrentAnimatorStateInfo(0).IsName("AttackCombo2")) {
			cayadoCollider.enabled = true;
			StartCoroutine (SlowSpeed());
			attackCombo2 = false;
		}
		if (attackCombo1) {
			anim.SetTrigger ("AttackCombo1");
			cayadoCollider.enabled = true;
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("AttackCombo1")){
				StartCoroutine (SlowSpeed());
			}
			attackCombo1 = false;
		}

		anim.SetBool ("AttackCombo2", attackCombo2);

        _actSpeed = (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal")));
        anim.SetFloat("speed", _actSpeed);
    }
    void HandleAirMovement()
    {


        if (_isGrounded)
        {
           
         
            if (((Input.GetButtonDown("Jump")) || (Input.GetButtonDown("X_PS4"))) && !isRolling)
            {
                _moveDirection.y = jumpForce;
                anim.SetBool("IsGrounded", _isGrounded);
            }
        }
        else
        {
            _moveDirection.y = _moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        }
        
    }
    void HandleGroundedMovement()
    {
        if (isRolling)
        {
            Vector3 rollDirection = playerModel.transform.forward * rollSpeed;
            _characterController.Move(rollDirection * Time.deltaTime);
        }
        else { 
            float yStore = _moveDirection.y;
            _moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
            //normalize the player movement so diagonal movement is not twice as fast as single axis movement
            _moveDirection = (_moveDirection.magnitude > 1) ? _moveDirection.normalized * moveSpeed : _moveDirection * moveSpeed;
            _moveDirection.y = yStore;
            _characterController.Move(_moveDirection * Time.deltaTime);
        }
        HandleRolling();
    }
    void HandleGuardianMode()
    {
        if ( (Input.GetKeyDown(KeyCode.G)) || (Input.GetButtonDown("L1_PS4")) )
        {
            _cambiandoModo = true;
        }

        if (_cambiandoModo)
        {
            if (_lerpTime <= 1)
            {
                if (modoGuardian)
                {
                    playerMesh.GetComponent<Renderer>().material.mainTexture = textures[0];
                    AudioManager.instance.PlaySound("ModoGuardian");
                }
                else
                {
                    playerMesh.GetComponent<Renderer>().material.mainTexture = textures[1];
                    AudioManager.instance.PlaySound("ModoSaru");

                }
                _lerpTime += Time.deltaTime * _lerpSpeed;
                _rgbColorFilter.ColorRGB = (!modoGuardian) ? Color.Lerp(_colorModoSaru, _colorModoGuardian, _lerpTime) : Color.Lerp(_colorModoGuardian, _colorModoSaru, _lerpTime);
                _anomalyFilter.Intensity = (!modoGuardian) ? Mathf.Lerp(0, 0.2f, _lerpTime) : Mathf.Lerp(0.2f, 0, _lerpTime);
            }
            else if (_lerpTime > 1)
            {
                modoGuardian = !modoGuardian;
                _cambiandoModo = false;
                _lerpTime = 0;
            }

        }
    }
    void HandleRolling()
    {
        if ( Input.GetButtonDown("O_PS4") && _actSpeed >= 0.6f && !isRolling && _isGrounded)  
        {
            Debug.Log("Actual Speed = " + _actSpeed);
            StartCoroutine(RollRoutine());
        }
    }
    void HandleAttacking()
    {
		if ((Input.GetKeyDown (KeyCode.E)) || (Input.GetButtonDown ("Cuadrado_PS4"))) {
			if (modoGuardian || !canAttack)
				return;

			if (anim.GetCurrentAnimatorStateInfo(0).IsName("AttackCombo1")){
				attackCombo2 = true;
			}

			attackCombo1 = true;
					
			isAttacking = true;
        }
    }

    public IEnumerator RollRoutine()
    {
        isRolling = true;
        float auxSpeed = moveSpeed;
        _characterController.height -= 0.5f;
        _characterController.center -= new Vector3(0f, 0.25f, 0f);
        moveSpeed = rollSpeed;
        yield return new WaitForSeconds(0.75f);
        moveSpeed = auxSpeed;
        _characterController.center = Vector3.zero;
        _characterController.height += 0.5f;
        isRolling = false;
    }

    public IEnumerator AttackRoutine()
    {
        isAttacking = true;
        float auxSpeed = moveSpeed;
        moveSpeed = 0;
        yield return new WaitForSeconds(1.4f);
        moveSpeed = auxSpeed;
        isAttacking = false;
    }
		
	IEnumerator SlowSpeed(){
		moveSpeed = maxSpeed/2;
		yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo(0).length);
		cayadoCollider.enabled = false;
		moveSpeed = maxSpeed;
	}

}

using System;
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
    public bool modoGuardianActivado = false;

    public Texture[] textures;
    public GameObject playerMesh;

    private CharacterController _characterController;
    private Vector3 _moveDirection;
    private float _actSpeed;
    public bool isRolling;
    public bool isDead = false;
    public bool stop = false;

    [HideInInspector]
    public bool isAttacking;
	private bool attackCombo1 = false;
	private bool attackCombo2 = false;
	private bool attackCombo2Time = true;
	public bool canAttack = false;
	public BoxCollider cayadoCollider;

    private float _lerpSpeed = 2f;
    private Color _colorModoSaru = new Color(1, 1, 1, 1);
    private Color _colorModoGuardian = new Color(0.58f, 0.33f, 0.87f, 1);
    private float _lerpTime = 0;
    private bool _isGrounded = true;
    private bool _lastFrameGrounded;
    private float initialSpeed;
    

    private CameraFilterPack_Color_RGB _rgbColorFilter;
    private CameraFilterPack_3D_Anomaly _anomalyFilter;

    public bool cambiandoModo = false;

	public bool canDoThings = true;

    //el layer dentro del animator para el modo mono
    const int saruModeLayer = 0;
    //el layer dentro del animator para el modo guardian
    const int guardianModeLayer = 1;
    #endregion

    void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _rgbColorFilter = cam.GetComponent<CameraFilterPack_Color_RGB>();
        _anomalyFilter = cam.GetComponent<CameraFilterPack_3D_Anomaly>();

        _rgbColorFilter.ColorRGB = _colorModoSaru;
        _anomalyFilter.Intensity = 0;
		maxSpeed = moveSpeed;
        initialSpeed = maxSpeed;
	}

    void Update()
    {
		if (!canDoThings)
			return;

        HandleIsGrounded();
        HandleGroundedMovement();
        HandleAirMovement();
        HandlePlayerRotation();
		HandleAttacking();
        HandleAnimations();
        if (modoGuardianActivado)
        {
            HandleGuardianMode();
        }
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
            //Debug.Log(hit.transform.name);
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
        anim.SetBool("IsDead", isDead);

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
        else if (!stop)
        {
            float yStore = _moveDirection.y;
            _moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
            //normalize the player movement so diagonal movement is not twice as fast as single axis movement
            _moveDirection = (_moveDirection.magnitude > 1) ? _moveDirection.normalized * moveSpeed : _moveDirection * moveSpeed;
            _moveDirection.y = yStore;
            _characterController.Move(_moveDirection * Time.deltaTime);
        }
        else {
            float yStore = _moveDirection.y;
            _moveDirection = transform.up * yStore;
            _characterController.Move(_moveDirection * Time.deltaTime);

        }
        HandleRolling();
    }
    void HandleGuardianMode()
    {
        if ( (Input.GetKeyDown(KeyCode.G)) || (Input.GetButtonDown("L1_PS4")) && PlayerManager.instance.currentEnergy > 0)
        {
            cambiandoModo = true;
           
            //anim.SetTrigger("Change");
            //anim.SetBool("ChangeMode", modoGuardian);
         
        }

        if (cambiandoModo)
        {
            if (_lerpTime <= 1)
            {
                if (modoGuardian)
                {
                    playerMesh.GetComponent<Renderer>().material.mainTexture = textures[0];
                    playerMesh.GetComponent<Renderer>().materials[1].SetFloat("OKARU", 0);
                    anim.SetLayerWeight(saruModeLayer, _lerpTime);
                    anim.SetLayerWeight(guardianModeLayer, 1-_lerpTime);
                }
                else
                {
                    playerMesh.GetComponent<Renderer>().material.mainTexture = textures[1];
                    playerMesh.GetComponent<Renderer>().materials[1].SetFloat("OKARU", 1);
                    anim.SetLayerWeight(saruModeLayer, 1 - _lerpTime);
                    anim.SetLayerWeight(guardianModeLayer,  _lerpTime);
                }
                _lerpTime += Time.deltaTime * _lerpSpeed;
                _rgbColorFilter.ColorRGB = (!modoGuardian) ? Color.Lerp(_colorModoSaru, _colorModoGuardian, _lerpTime) : Color.Lerp(_colorModoGuardian, _colorModoSaru, _lerpTime);
                _anomalyFilter.Intensity = (!modoGuardian) ? Mathf.Lerp(0, 0.2f, _lerpTime) : Mathf.Lerp(0.2f, 0, _lerpTime);
            }
            else if (_lerpTime > 1)
            {
                if (modoGuardian)
                {
                    AudioManager.instance.PlaySound("ModoGuardian");
                    anim.SetLayerWeight(saruModeLayer, 1);
                    anim.SetLayerWeight(guardianModeLayer, 0);

                }
                else
                {
                    AudioManager.instance.PlaySound("ModoSaru");
                    anim.SetLayerWeight(saruModeLayer, 0);
                    anim.SetLayerWeight(guardianModeLayer, 1);
                }

                modoGuardian = !modoGuardian;
                if (modoGuardian == true)
                {
                    moveSpeed = maxSpeed * 1.25f;
                    PlayerManager.instance.CancelInvoke("RestoreEnergy");
                    PlayerManager.instance.InvokeRepeating("DepleteEnergy", Time.deltaTime, Time.deltaTime);
                }
                else
                {
                    moveSpeed = initialSpeed;
                    PlayerManager.instance.CancelInvoke("DepleteEnergy");
                    PlayerManager.instance.InvokeRepeating("RestoreEnergy", Time.deltaTime, Time.deltaTime);

                }
                cambiandoModo = false;
                _lerpTime = 0;
                UIManager.instance.ChangeHUD();
            }
        }

        if (PlayerManager.instance.currentEnergy <= 0)
        {
            cambiandoModo = true;
            PlayerManager.instance.CancelInvoke("DepleteEnergy");
            PlayerManager.instance.CancelInvoke("RestoreEnergy");
            PlayerManager.instance.InvokeRepeating("RestoreEnergy", Time.deltaTime, Time.deltaTime);
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

	public void FireAttackAnim(){
		anim.SetTrigger ("FireAttack");
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public class playerControllerCustom : MonoBehaviour
{
    public GameObject target;
    #region variables   
    //los valores de down y up se multiplican para obtener un thresold correcto
    private enum EnemyPosition { up, down, left, right, close }
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
    [HideInInspector]
    public int numberOfAttack = 1;
	public bool canAttack = false;
	public BoxCollider cayadoCollider;
    [SerializeField]public List<Transform> enemiesClose;
    private float _lerpSpeed = 2f;
    private Color _colorModoSaru = new Color(1, 1, 1, 1);
    private Color _colorModoGuardian = new Color(0.58f, 0.33f, 0.87f, 1);
    private float _lerpTime = 0;
    private bool _isGrounded = true;
    private bool _lastFrameGrounded;
    private float initialSpeed;
    [SerializeField]
    private bool _battleMode;
    private float _thresholdEnemy = 200;
    [SerializeField]
    private float _thresholdLeftJoystick = .5f;
    [SerializeField]
    private float _timeToTargetNewEnemy = .5f;
    private float _actualTimeTargetEnemyWithDirection;
    private Transform _enemyTarget = null;
    [SerializeField]
    private CinemachineVirtualCamera _cameraBattleMode;
    private CinemachineBrain _cameraBrain;

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
        _cameraBrain = cam.GetComponent<CinemachineBrain>();
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
        if (_battleMode)
        {
            HandleBattleMode();
        }
        HandleAttacking();
        HandleAnimations();
        if (modoGuardianActivado)
        {
            HandleGuardianMode();
        }
        
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

		//anim.SetBool ("AttackCombo2",attackCombo2);

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
        if ((Input.GetButtonDown("R3_PS4")) && (enemiesClose.Count > 0))
        {
            if (_battleMode)
            {
                ExitBattleMode();
            }else{
                EnterBattleMode();
            }
            
        }
        if ((Input.GetKeyDown (KeyCode.E)) || (Input.GetButtonDown ("Cuadrado_PS4"))) {
			if (modoGuardian || !canAttack)
				return;

			if (numberOfAttack==2){
                anim.SetTrigger("AttackCombo2");
            }
            else
            {
                anim.SetTrigger("AttackCombo1");
            }		
			isAttacking = true;
        }
    }

    void HandleBattleMode()
    {
        if (Input.GetAxis("Pad_Derecho_PS4_Horizontal") > _thresholdLeftJoystick && _actualTimeTargetEnemyWithDirection <= 0)
        {
            NewTarget(EnemyPosition.left);
        }
        else if (Input.GetAxis("Pad_Derecho_PS4_Horizontal") < - _thresholdLeftJoystick && _actualTimeTargetEnemyWithDirection <= 0)
        {
            NewTarget(EnemyPosition.right);          
        }
        else if (Input.GetAxis("Pad_Derecho_PS4_Vertical") > _thresholdLeftJoystick && _actualTimeTargetEnemyWithDirection <= 0)
        {
            NewTarget(EnemyPosition.up);
        }
        else if (Input.GetAxis("Pad_Derecho_PS4_Vertical") < -_thresholdLeftJoystick && _actualTimeTargetEnemyWithDirection <= 0)
        {
            NewTarget(EnemyPosition.down);
        }
        else if(_actualTimeTargetEnemyWithDirection > 0)
        {
            _actualTimeTargetEnemyWithDirection -= Time.deltaTime;
        }
        Quaternion lookRotation = Quaternion.LookRotation(_enemyTarget.position - playerModel.transform.position);
        playerModel.transform.rotation = new Quaternion(0f, lookRotation.y, 0f, lookRotation.w);
    }

    //funcion que swithchea dependiendo del inputrecibido para  targetear enemigo
    void NewTarget(EnemyPosition findPosition)
    {
        if (enemiesClose.Count == 0)
        {
            ExitBattleMode();
            return;
        }
        _thresholdEnemy = 200;
        switch (findPosition)
        {
            case EnemyPosition.right:
                TargetNewEnemyLeftOrRight(findPosition);
                break;
            case EnemyPosition.left:
                TargetNewEnemyLeftOrRight(findPosition);
                break;
            case EnemyPosition.close:
                TargetNewEnemy();
                break;
            case EnemyPosition.up:
                TargetNewEnemyUpOrDown(findPosition);
                break;
            case EnemyPosition.down:
                TargetNewEnemyUpOrDown(findPosition);
                break;
            default:
                break;
        }
    }


    //obtiene el enemigo mas cercano
    //se utiliza cuando se targetea y no es  por movimiento del stick derecho
    void TargetNewEnemy()
    {
        Transform actualTarget = _enemyTarget;
        float distance;
        foreach (Transform enemy in enemiesClose)
        {
            if (actualTarget == enemy)
                continue;
            //calculamos el vector hacia donde esta el enemigo desde la posicion de Saru   
            distance = (transform.position - enemy.position).magnitude;
            if (_thresholdEnemy > distance)
            {
                NextTarget(distance, enemy);
            }         
        }
    }
    //obtiene el enemigo mas cercano dependiendo del movimiento derecha o izquierda del  recibido
    void TargetNewEnemyUpOrDown(EnemyPosition findPosition)
    {
        Transform actualTarget = _enemyTarget;
        //variable necesaria para que el mismo codigo funcione para up y para down
        int upOrDown = findPosition == EnemyPosition.up? -1 : 1 ;
      
        _thresholdEnemy = (transform.position - _enemyTarget.position).magnitude * upOrDown;
        
        float distance;
        float lastDistance = -100;
        foreach (Transform enemy in enemiesClose)
        {
            if (actualTarget == enemy)
                continue;
            //calculamos el vector hacia donde esta el enemigo desde la posicion de Saru   
            distance = (transform.position - enemy.position).magnitude * upOrDown;
            if ((distance < _thresholdEnemy) && (distance > lastDistance) )
            {
                NextTarget(distance, enemy);
                lastDistance = distance;
                _actualTimeTargetEnemyWithDirection = _timeToTargetNewEnemy;
            }
            
        }
        if (actualTarget != _enemyTarget)
        {
            _actualTimeTargetEnemyWithDirection = _timeToTargetNewEnemy;
            target.transform.localScale = new Vector3(.2f, .2f, .2f);
        }
    }

    //obtiene el enemigo mas cercano dependiendo del movimiento derecha o izquierda del  recibido
    void TargetNewEnemyLeftOrRight(EnemyPosition findPosition)
    {
        //calculamos un punto donde mira saru para poder calcular su vector
        Vector2 positionLook = new Vector2(playerModel.transform.forward.x + transform.position.x, playerModel.transform.forward.z + transform.position.z);
        //calculamos el vector hacia donde mira saru desde su ubicacion
        Vector2 actualLookAt = new Vector2(positionLook.x - transform.position.x, positionLook.y - transform.position.z);
        //esta variable sirve para guardar el enemigo que tenemos tarjeteado antes de comprobar todos ya que _enemytarget cambia dentro del foreach
        Transform actualTarget = _enemyTarget;
        foreach (Transform enemy in enemiesClose)
        {
            if (actualTarget == enemy)
                continue;
            //calculamos el vector hacia donde esta el enemigo desde la posicion de Saru   
            Vector2 enemyPlayerPosition = new Vector2(enemy.position.x - transform.position.x, enemy.position.z - transform.position.z);
            float leftOrRight = actualLookAt.x * enemyPlayerPosition.y - actualLookAt.y * enemyPlayerPosition.x;
            if ((leftOrRight > 0 && findPosition == EnemyPosition.right) || (leftOrRight < 0 && findPosition == EnemyPosition.left))
            {
                float threshold = Vector2.Angle(actualLookAt, enemyPlayerPosition);
                if (_thresholdEnemy > threshold)
                {
                    NextTarget(leftOrRight, enemy);
                }
            }
        }
        if (actualTarget != _enemyTarget)
        {
            _actualTimeTargetEnemyWithDirection = _timeToTargetNewEnemy;
        }
    }

    //determina el siguiente objetivo al que seleccionar;
    void NextTarget(float threshold, Transform enemy)
    {
        _thresholdEnemy = threshold;
        _enemyTarget = enemy;
        //esto es solo para testeo
        target.transform.parent = _enemyTarget.GetChild(0).GetChild(0);
        target.transform.localPosition = new Vector3(-0.5f,0f,0f);
    }

    void EnterBattleMode()
    {
        _cameraBrain.SetCameraOverride(1, PlayerManager.instance.cameraCinemachine, _cameraBattleMode, 1, Time.deltaTime);
        _battleMode = true;
        TargetNewEnemy();
        target.SetActive(true);
    }
    void ExitBattleMode()
    {
        _cameraBrain.SetCameraOverride(1, _cameraBattleMode,PlayerManager.instance.cameraCinemachine, 1, Time.deltaTime);
        _battleMode = false;
        _thresholdEnemy = 200;
        _enemyTarget = null;
        target.SetActive(false);
    }

    public void EnemyExitRange(Transform enemyTransform)
    {
        enemiesClose.Remove(enemyTransform);
        if (_enemyTarget != null && _enemyTarget == enemyTransform)
        {
            TargetNewEnemy();          
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

    public void EnterAttack(){
        numberOfAttack++;
		moveSpeed = maxSpeed/2;
		cayadoCollider.enabled = true;
        isAttacking = true;
    }

    public void ExitAttack()
    {
        moveSpeed = maxSpeed;
        cayadoCollider.enabled = false;
        isAttacking = false;
    }

	public void FireAttackAnim(){
		anim.SetTrigger ("FireAttack");
	}
}

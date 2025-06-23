using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Events;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.LowLevel;


namespace Player
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour, Health
    {

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        [Range(0, 10)]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        [Range(0, 20)]
        public float SprintSpeed = 6.0f;
        private bool canSprint = true;
        [Tooltip("Rotation speed of the character")]
        [Range(0, 5)]
        public float RotationSpeed = 1.0f;
        [SerializeField] private float MouseRotationSpeed = 1.0f;
        [SerializeField] private float GamepadRotationSpeed = 0.4f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        public bool canMove = true;
        public bool canRotate = true;
        [SerializeField]
        private PlayerInput playerInput;
        private bool isInCenter;
        [SerializeField] private bool tuto;


        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        public LayerMask InteractLayers;
        private Vector2 lastMousePosition;
        private const float joystickDeadzone = 0.1f;



        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]

        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;
        private bool isOnGamepad = false;


        [Header("Weapon")]
        [SerializeField]
        private Animator weaponAnim;
        [SerializeField]
        private PoolBullet poolBullets;
        private bool isAiming;
        [SerializeField]
        private float shotCouldown;
        private float timeSinceLastBullet;
        [SerializeField] private int maxBulletInMagazine;
        private int bulletLeftInMagazine;
        [SerializeField] private float reloadTime;
        private float reloadTimeLeft;
        [SerializeField]
        private Transform bulletLauncher;
        private Coroutine reloadCoroutine;

        [Header("Turret")]
        [SerializeField]
        private GameObject turretProjection;
        [SerializeField]
        private float turretPlacementDistance;
        [SerializeField]
        private TurretBehaviour sniper;
        [SerializeField]
        private TurretBehaviour machineGun;

        [Header("HEALTH")]
        [SerializeField]
        private float maxHealth = 100;
        [SerializeField]
        private float currentHealth;
        [SerializeField]
        private Animator anim;
        [SerializeField]
        private Animator getHit;
        [SerializeField]
        private float hitAudioCouldown;
        private Coroutine hitCoroutine;
        [SerializeField]
        private float deathAnimationDuration = 6;
        [SerializeField]
        private Image healthFilter;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Animator healthBarAnim;
        UnityEvent getHealed;

        [Header("Inventory")]
        [SerializeField]
        private InventorySystem inventory;
        [SerializeField]
        private GameObject turretSniper;
        [SerializeField]
        private GameObject turretMachineGun;

        [Header("Interaction")]
        [SerializeField]
        private float interactionDistance = 5;
        [SerializeField]
        private float minningStrenght;
        private bool interacting;
        private bool minning;
        private bool crafting;
        [SerializeField]
        private float audioInterval;
        private Coroutine audioCoroutine;
        FragmentBehaviour fragment;
        DesktopType desktop;
        [SerializeField]
        private InputAction interactAction;
        private bool turretInHand;
        UnityEvent getCrystal, saveCrystal;
        public bool canInteractWithGenerator = true, canInteractWithTransfert = true, canInteractWithDesktop = true, canInteractWithHeal = true;

        [Header("UI")]
        [SerializeField]
        private Animator notEnoughtMessage;
        [SerializeField]
        private Animator notEnoughtCrystal;
        [SerializeField]
        private Animator crosshairAnim;
        [SerializeField]
        private GameObject redCrosshair;
        [SerializeField]
        private Slider minningSlider;
        [SerializeField]
        private GameObject littleFragSteps;
        [SerializeField]
        private GameObject mediumFragSteps;
        [SerializeField]
        private GameObject bigFragSteps;
        [SerializeField]
        private MenuInGame menuInGame;
        [HideInInspector]
        public bool inMenu = false;

        [Header("Stats On Death")]
        [SerializeField]
        private TextMeshProUGUI timeOnDeathTxt;
        [SerializeField]
        private TextMeshProUGUI waveOnDeathTxt;
        [SerializeField]
        private TextMeshProUGUI crystalOnDeathTxt;
        private float startTime;

        [Header("Audio")]
        [SerializeField]
        private bool playAudioGlitch;
        [SerializeField]
        private float stepCouldownWalk;
        [SerializeField]
        private float stepCouldownSprint;
        private float stepCouldown;
        private Coroutine stepCoroutine;

        [Header("Spit")]
        [SerializeField] SpitOnGroundBehaviour spit;
        [SerializeField] Animator spitAnim;
        bool isSpitted = false;

        // cinemachine
        private float _cinemachineTargetPitch;


        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            #region Cursor + Window
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Cursor Visible: " + Cursor.visible + " | LockState: " + Cursor.lockState);
            #endregion

            #region Save/Load Data
            GameData gameData = SaveSystem.Load();
            if (gameData == null)
            {
                Debug.LogError("Can't load save data");
            }
            else
            {
                print(gameData.upgradesUnlocked.Count + " upgrades");
                foreach (UpgradeDataSave upgradeSaved in gameData.upgradesUnlocked)
                {
                    switch (upgradeSaved.type)
                    {
                        case UpgradeType.Heal:
                            maxHealth += upgradeSaved.incrementValue;
                            Debug.Log("Health upgrade detected with the value of " + upgradeSaved.incrementValue);
                            break;


                        case UpgradeType.Damage:
                            poolBullets.bulletPrefab.AddDamage(upgradeSaved.incrementValue);
                            Debug.Log("Damage upgrade detected with the value of " + upgradeSaved.incrementValue);
                            break;


                        case UpgradeType.Speed:
                            _speed += upgradeSaved.incrementValue;
                            Debug.Log("Speed upgrade detected with the value of " + upgradeSaved.incrementValue);
                            break;
                    }
                }
            }
            #endregion

            #region Get Component
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            #endregion

            #region Set data
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            timeSinceLastBullet = shotCouldown;
            currentHealth = maxHealth;
            UpdateHealthBar();
            playerInput.actions["Interact"].canceled += StopInteract;
            playerInput.actions["InteractWithGamepad"].canceled += StopInteract;
            InputSystem.onAnyButtonPress.Call(OnAnyInput);
            InputSystem.onEvent += OnInputEvent;
            startTime = Time.time;
            bulletLeftInMagazine = maxBulletInMagazine;
            getCrystal = new UnityEvent();
            getHealed = new UnityEvent();
            saveCrystal = new UnityEvent();
            littleFragSteps.gameObject.SetActive(false);
            mediumFragSteps.gameObject.SetActive(false);
            bigFragSteps.gameObject.SetActive(false);
            #endregion

            #region OOP
            GeneratorBehaviour.instance.SubOutOfPower(OutOfPower);
            GeneratorBehaviour.instance.SubPowerBack(PowerBack);
            crosshairAnim.SetBool("Power", true);
            notEnoughtCrystal.SetBool("PowerOn", true);
            healthBarAnim.SetBool("PowerOn", true);
            #endregion

        }

        private void Update()
        {
            if (playAudioGlitch)
            {
                playAudioGlitch = false;
                AudioManager.instance.PlayAudio(transform, "Glitch", 0.25f, Random.Range(0.95f, 1.05f));
            }

            #region Player Actions
            JumpAndGravity();
            GroundedCheck();
            if (canMove) Move();

            if (canRotate) Aim();
            if (interacting && minning) { Minning(); }
            else if (interacting && crafting) { Crafting(); }

            #endregion

            #region Reset all actions
            else // No action
            {
                interacting = false;
                minning = false;
                if (!isSpitted) canMove = true;
                if (!inMenu) canRotate = true;
                minningSlider.gameObject.SetActive(false);
                littleFragSteps.gameObject.SetActive(false);
                mediumFragSteps.gameObject.SetActive(false);
                bigFragSteps.gameObject.SetActive(false);
            }
            timeSinceLastBullet += Time.deltaTime;
            #endregion

            #region raycast interract
            RaycastHit hitInterraction;

            if (Physics.Raycast(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward), out hitInterraction, interactionDistance, InteractLayers))
            {
                if (hitInterraction.collider.CompareTag("generator")) // INTERACT WITH GENERATOR
                {
                    if (inventory.GetFragmentQuantity() <= 0)
                    {
                        //notEnoughtMessage.SetActive(true);
                    }
                    //else notEnoughtMessage.SetActive(false);
                }
                //else notEnoughtMessage.SetActive(false);
            }
            //else notEnoughtMessage.SetActive(false);

            #endregion

            #region raycast turret placement
            RaycastHit hitTurretPlacement;

            if (inventory.GetItemInHand().CompareTag("turret"))
            {
                turretProjection = inventory.GetItemInHand();
                Vector3 turretPos;
                if (Physics.Raycast(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward), out hitTurretPlacement, turretPlacementDistance, InteractLayers))
                {
                    #region raycast hit 
                    if (!hitTurretPlacement.collider.CompareTag("Ground"))
                    {
                        RaycastHit hitDown;
                        if (Physics.Raycast(hitTurretPlacement.point, Vector3.down, out hitDown)) // Hit a wall so raycast to project turret on ground
                        {
                            turretPos = hitDown.point;
                        }
                        else turretPos = new Vector3(hitInterraction.point.x, 0, hitInterraction.point.z);
                    }
                    else // Hit the ground, so project turret on the hit point
                    {
                        turretPos = hitTurretPlacement.point;
                    }
                    #endregion
                }
                #region don't hit
                else if (Physics.Raycast(CinemachineCameraTarget.transform.position + (transform.forward * turretPlacementDistance), Vector3.down, out hitTurretPlacement)) // hit nothing so raycast at forward max distance to project the turret on the ground
                {
                    turretPos = hitTurretPlacement.point;
                }
                else
                {
                    Vector3 forwardMult = transform.forward * turretPlacementDistance;
                    turretPos = new Vector3(forwardMult.x + transform.position.x, 0, forwardMult.z + transform.position.z);
                }
                #endregion
                turretProjection.transform.position = turretPos;
                turretProjection.transform.parent.rotation = new Quaternion(0, turretProjection.transform.parent.rotation.y, 0, turretProjection.transform.parent.rotation.w);
            }
            #endregion

            if (currentHealth != maxHealth) healthFilter.color = new Color(healthFilter.color.r, healthFilter.color.g, healthFilter.color.b, 0.5f - (currentHealth / 100));
            else healthFilter.color = new Color(healthFilter.color.r, healthFilter.color.g, healthFilter.color.b, 0);
        }

        private void LateUpdate()
        {
            if (!canRotate) return;
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        #region primary movement
        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            if (inMenu) return;
            float targetSpeed = MoveSpeed;
            if (_input.sprint && canSprint)
                targetSpeed = SprintSpeed;

            if (targetSpeed == SprintSpeed)
            {
                stepCouldown = stepCouldownSprint;
            }
            else stepCouldown = stepCouldownWalk;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
                if (stepCoroutine == null && Grounded) stepCoroutine = StartCoroutine(PlayStepAudio());
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        }

        private void OnAnyInput(InputControl control)
        {
            if (control.device is Gamepad && RotationSpeed != GamepadRotationSpeed)
            {
                SwitchToGamepad();
            }
            else if (!(control.device is Gamepad) && RotationSpeed != MouseRotationSpeed)
            {
                SwitchToKeyboardMouse();
            }
        }

        private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (device is Mouse mouse)
            {
                Vector2 currentMousePos = mouse.position.ReadValue();
                if ((currentMousePos - lastMousePosition).sqrMagnitude > 0.01f)
                {
                    lastMousePosition = currentMousePos;
                    SwitchToKeyboardMouse();
                }
            }
            else if (device is Gamepad gamepad)
            {
                Vector2 leftStick = gamepad.leftStick.ReadValue();
                Vector2 rightStick = gamepad.rightStick.ReadValue();

                if (leftStick.sqrMagnitude > joystickDeadzone * joystickDeadzone ||
                    rightStick.sqrMagnitude > joystickDeadzone * joystickDeadzone)
                {
                    SwitchToGamepad();
                }
            }
        }

        private void SwitchToKeyboardMouse()
        {
            RotationSpeed = MouseRotationSpeed;
            isOnGamepad = false;
        }

        private void SwitchToGamepad()
        {
            RotationSpeed = GamepadRotationSpeed;
            isOnGamepad = true;
        }

        private IEnumerator PlayStepAudio()
        {
            if (isInCenter)
            {
                string[] step =
                {
                    "StepCenter1",
                    "StepCenter2",
                    "StepCenter3",
                    "StepCenter4",
                    "StepCenter5",
                    "StepCenter6"
                };
                AudioManager.instance.PlayRandomAudio(transform, step, 0.8f, Random.Range(0.8f, 0.9f));
            }
            else AudioManager.instance.PlayAudio(transform, "StepMine", 0.8f, Random.Range(0.7f, 0.9f));
            yield return new WaitForSeconds(stepCouldown);
            stepCoroutine = null;
        }

        private void JumpAndGravity()
        {
            if (inMenu) return;
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        #endregion

        #region Weapon

        private void Aim()
        {
            if (inMenu) return;
            if (isAiming != _input.aim && inventory.GetItemInHand().name == "Gun")
            {
                isAiming = _input.aim;
                weaponAnim.SetBool("Aim", isAiming);
                crosshairAnim.SetBool("Aiming", isAiming);
            }
        }

        private void OnShot()
        {
            if (inMenu) return;
            if (timeSinceLastBullet >= shotCouldown && inventory.GetItemInHand().name == "Gun" && currentHealth > 0)
            {
                if (bulletLeftInMagazine > 0)
                {
                    BulletBehaviour bullet = poolBullets.GetBullet();
                    AudioManager.instance.PlayAudio(transform, "Shot", 0.5f, Random.Range(0.9f, 1.1f));
                    weaponAnim.SetTrigger("Shot");
                    timeSinceLastBullet = 0;
                    bullet.Launch();
                    bulletLeftInMagazine--;
                    if (bulletLeftInMagazine <= 0)
                    {
                        canSprint = false;
                        reloadCoroutine = StartCoroutine(WaitReloadTimer());
                    }
                }
            }
            else if (inventory.GetItemInHand().name == "Sniper" || inventory.GetItemInHand().name == "MachineGun")
            {
                OnTurretPlacement();
                AudioManager.instance.PlayAudio(transform, "TurretDeployed", 0.75f);
            }
        }

        public void BulletTouch()
        {
            redCrosshair.SetActive(true);
            crosshairAnim.SetTrigger("Touch");
        }

        private void OnReload()
        {
            if (inMenu) return;
            if (reloadCoroutine != null) return;
            if (bulletLeftInMagazine < maxBulletInMagazine && currentHealth > 0)
            {
                bulletLeftInMagazine = 0;
                canSprint = false;
                reloadCoroutine = StartCoroutine(WaitReloadTimer());
            }
        }

        private IEnumerator WaitReloadTimer()
        {
            AudioManager.instance.PlayAudio(transform, "Reload");
            yield return new WaitForSeconds(shotCouldown);
            weaponAnim.SetTrigger("Reload");
            yield return new WaitForSeconds(reloadTime);
            bulletLeftInMagazine = maxBulletInMagazine;
            canSprint = true;
            reloadCoroutine = null;
        }

        #endregion

        #region Turret

        private void OnTurretPlacement()
        {
            switch (inventory.GetItemInHand().gameObject.name)
            {
                case "Sniper":
                    TurretBehaviour sniper = PoolTurret.instance.GetTurret(turretProjection.transform.position, TurretType.sniper);
                    sniper.transform.position = turretProjection.transform.position;
                    inventory.RemoveItem(inventory.GetItemInHand());
                    Debug.Log("sniper created");
                    break;

                case "MachineGun":
                    TurretBehaviour machineGun = PoolTurret.instance.GetTurret(turretProjection.transform.position, TurretType.mahineGun);
                    machineGun.transform.position = turretProjection.transform.position;
                    inventory.RemoveItem(inventory.GetItemInHand());
                    Debug.Log("machine gun created");
                    break;

                default:
                    Debug.Log("Error : Turret name not found");
                    break;
            }
        }

        #endregion

        private void OnScroll()
        {
            if (inMenu) return;
            inventory.Scroll(_input.newWeapon);
        }


        #region Interaction


        private void OnInteract()
        {
            if (inMenu) return;
            RaycastHit hit;
            interacting = true;
            Debug.DrawRay(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward) * interactionDistance, Color.green, 100);

            if (Physics.Raycast(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward), out hit, interactionDistance, InteractLayers))
            {
                // Action depending on raycast data
                switch (hit.collider.tag)
                {
                    case "generator":
                        if (!canInteractWithGenerator) return;
                        GeneratorBehaviour generator = hit.collider.gameObject.GetComponent<GeneratorBehaviour>();
                        if (generator != null)
                        {
                            if (inventory.RemoveFragment(1))
                            {
                                generator.AddFragment(1);
                            }
                            else
                            {
                                notEnoughtMessage.SetTrigger("Show");
                                notEnoughtCrystal.SetTrigger("NotEnought");
                            }
                        }
                        break;

                    case "desktop":
                        if (!canInteractWithDesktop) return;
                        desktop = hit.collider.gameObject.GetComponentInParent<DesktopType>();
                        if (inventory.GetFragmentQuantity() >= desktop.GetFragmentNeeded())
                        {
                            crafting = true;
                            canMove = false;
                            canRotate = false;
                        }
                        else
                        {
                            notEnoughtMessage.SetTrigger("Show");
                            notEnoughtCrystal.SetTrigger("NotEnought");
                        }
                        break;

                    case "turret":
                        TurretBehaviour turret = hit.collider.gameObject.GetComponentInParent<TurretBehaviour>();

                        switch (turret.GetType())
                        {
                            case TurretType.sniper:
                                inventory.AddItem(turretSniper);
                                break;

                            case TurretType.mahineGun:
                                inventory.AddItem(turretMachineGun);
                                break;

                            default:
                                Debug.Log("Turret name not found");
                                break;
                        }
                        PoolTurret.instance.AddTurretToPool(turret);
                        break;

                    case "fragment":
                        fragment = hit.collider.gameObject.GetComponentInParent<FragmentBehaviour>();
                        interacting = true;
                        minning = true;
                        canMove = false;
                        canRotate = false;
                        break;

                    case "FragmentTransfert":
                        if (!canInteractWithTransfert) return;
                        if (inventory.GetFragmentQuantity() > 0)
                        {
                            saveCrystal.Invoke();
                            int crystalQTT = SaveSystem.Load().crystalQuantity;
                            crystalQTT += 1;
                            inventory.RemoveFragment(1);
                            SaveSystem.SetCrystalQuantity(crystalQTT);
                            FragmentTransfert.instance.AddCrystalSaved(1);
                        }
                        else
                        {
                            notEnoughtMessage.SetTrigger("Show");
                            notEnoughtCrystal.SetTrigger("NotEnought");
                        }
                        break;

                    case "HealMachine":
                        if (!canInteractWithHeal) return;
                        HealMachine healMachine = hit.collider.gameObject.GetComponent<HealMachine>();
                        if (healMachine != null)
                        {
                            switch (healMachine.GetStat())
                            {
                                case HealMachineStats.Empty:
                                    if (inventory.RemoveFragment(1))
                                    {
                                        healMachine.Crafting();
                                    }
                                    else
                                    {
                                        notEnoughtMessage.SetTrigger("Show");
                                        notEnoughtCrystal.SetTrigger("NotEnought");
                                    }
                                    break;


                                case HealMachineStats.Crafting:

                                    break;


                                case HealMachineStats.Done:
                                    getHit.SetTrigger("Hit");
                                    AddHealth(healMachine.TakeHeal());
                                    AudioManager.instance.PlayAudio(transform, "Heal", 0.6f);
                                    break;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

        }

        private void OnInteractWithGamepad()
        {
            if (inMenu) return;
            RaycastHit hit;
            interacting = true;
            Debug.DrawRay(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward) * interactionDistance, Color.green, 100);

            if (Physics.Raycast(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward), out hit, interactionDistance, InteractLayers))
            {
                // Action depending on raycast data
                switch (hit.collider.tag)
                {
                    case "generator":
                        if (!canInteractWithGenerator) return;
                        GeneratorBehaviour generator = hit.collider.gameObject.GetComponent<GeneratorBehaviour>();
                        if (generator != null)
                        {
                            if (inventory.RemoveFragment(1))
                            {
                                generator.AddFragment(1);
                            }
                            else
                            {
                                notEnoughtMessage.SetTrigger("Show");
                                notEnoughtCrystal.SetTrigger("NotEnought");
                            }
                        }
                        break;

                    case "desktop":
                        if (!canInteractWithDesktop) return;
                        desktop = hit.collider.gameObject.GetComponentInParent<DesktopType>();
                        if (inventory.GetFragmentQuantity() >= desktop.GetFragmentNeeded())
                        {
                            crafting = true;
                            canMove = false;
                            canRotate = false;
                        }
                        else
                        {
                            notEnoughtMessage.SetTrigger("Show");
                            notEnoughtCrystal.SetTrigger("NotEnought");
                        }
                        break;

                    case "turret":
                        TurretBehaviour turret = hit.collider.gameObject.GetComponentInParent<TurretBehaviour>();

                        switch (turret.GetType())
                        {
                            case TurretType.sniper:
                                inventory.AddItem(turretSniper);
                                break;

                            case TurretType.mahineGun:
                                inventory.AddItem(turretMachineGun);
                                break;

                            default:
                                Debug.Log("Turret name not found");
                                break;
                        }
                        PoolTurret.instance.AddTurretToPool(turret);
                        break;

                    case "fragment":
                        fragment = hit.collider.gameObject.GetComponentInParent<FragmentBehaviour>();
                        interacting = true;
                        minning = true;
                        canMove = false;
                        canRotate = false;
                        break;

                    case "FragmentTransfert":
                        if (!canInteractWithTransfert) return;
                        if (inventory.GetFragmentQuantity() > 0)
                        {
                            saveCrystal.Invoke();
                            int crystalQTT = SaveSystem.Load().crystalQuantity;
                            crystalQTT += 1;
                            inventory.RemoveFragment(1);
                            SaveSystem.SetCrystalQuantity(crystalQTT);
                            FragmentTransfert.instance.AddCrystalSaved(1);
                        }
                        else
                        {
                            notEnoughtMessage.SetTrigger("Show");
                            notEnoughtCrystal.SetTrigger("NotEnought");
                        }
                        break;

                    case "HealMachine":
                        if (!canInteractWithHeal) return;
                        HealMachine healMachine = hit.collider.gameObject.GetComponent<HealMachine>();
                        if (healMachine != null)
                        {
                            switch (healMachine.GetStat())
                            {
                                case HealMachineStats.Empty:
                                    if (inventory.RemoveFragment(1))
                                    {
                                        healMachine.Crafting();
                                    }
                                    else
                                    {
                                        notEnoughtMessage.SetTrigger("Show");
                                        notEnoughtCrystal.SetTrigger("NotEnought");
                                    }
                                    break;


                                case HealMachineStats.Crafting:

                                    break;


                                case HealMachineStats.Done:
                                    getHit.SetTrigger("Hit");
                                    AddHealth(healMachine.TakeHeal());
                                    AudioManager.instance.PlayAudio(transform, "Heal", 0.6f);
                                    break;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            else
            {
                OnReload();
            }
        }

        private void StopInteract(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                interacting = false;
                canMove = true;
                canRotate = true;
                fragment = null;
            }
        }

        private void Minning()
        {
            if (inMenu) return;
            if (fragment == null)
            {
                minning = false;
                littleFragSteps.gameObject.SetActive(false);
                mediumFragSteps.gameObject.SetActive(false);
                bigFragSteps.gameObject.SetActive(false);
                return;
            }
            switch (fragment.type.quantity)
            {
                case 2: // Little crystal
                    littleFragSteps.gameObject.SetActive(true);
                    break;
                case 4: // Medium crystal
                    mediumFragSteps.gameObject.SetActive(true);
                    break;
                case 8: // Big crystal
                    bigFragSteps.gameObject.SetActive(true);
                    break;
                default: // None
                    Debug.LogWarning("Crystal with " + fragment.type.quantity + " crystals aren't adapted !");
                    break;
            }
            inventory.AddFragment(fragment.Hit(minningStrenght));
            string[] name = {
                "Minning1",
                "Minning2",
                "Minning3",
                "Minning4",
                "Minning5"
            };
            if (audioCoroutine == null) audioCoroutine = StartCoroutine(WaitForAudio(name));
            minningSlider.gameObject.SetActive(true);
            minningSlider.value = fragment.GetHealthValue();
            if (fragment.GetHealth() <= 0)
            {
                //Gain crystal
                getCrystal.Invoke();
                minningSlider.gameObject.SetActive(false);
                minning = false;
                littleFragSteps.gameObject.SetActive(false);
                mediumFragSteps.gameObject.SetActive(false);
                bigFragSteps.gameObject.SetActive(false);
            }
        }
        private IEnumerator WaitForAudio(string[] name)
        {
            if (name.Length == 1) AudioManager.instance.PlayAudio(transform, name[0], 0.5f, Random.Range(0.9f, 1.1f));
            else AudioManager.instance.PlayRandomAudio(transform, name, 0.5f, Random.Range(0.9f, 1.1f));
            yield return new WaitForSeconds(audioInterval);
            audioCoroutine = null;
        }

        private void Crafting()
        {
            if (inMenu) return;

            if (desktop == null)
            {
                crafting = false;
                return;
            }
            string[] name = { "Crafting" };
            if (audioCoroutine == null) audioCoroutine = StartCoroutine(WaitForAudio(name));
            if (desktop.Craft(inventory))
            {
                switch (desktop.turretType)
                {
                    case TurretType.sniper:
                        inventory.AddItem(turretSniper);
                        break;

                    case TurretType.mahineGun:
                        inventory.AddItem(turretMachineGun);
                        break;

                    default:
                        Debug.Log("Turret name not found");
                        break;
                }
                crafting = false;
            }
        }



        #endregion

        #region Menu

        private void OnEscape()
        {
            if (inMenu) menuInGame.OnResume();
            else menuInGame.Active();
            if(isOnGamepad)
            {
                StartCoroutine(menuInGame.GamepadActivation());
            }
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        #region Health

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;
            currentHealth -= damage;
            UpdateHealthBar();
            if (currentHealth <= 0)
            {
                Die();
            }
            getHit.SetTrigger("Hit");
            if (GeneratorBehaviour.instance.GetEnergy() > 0) healthBarAnim.SetTrigger("GetHit");
            if (hitCoroutine == null) hitCoroutine = StartCoroutine(TakeDamageCouldown());
        }

        private IEnumerator TakeDamageCouldown()
        {
            string[] names = {
                "Hit1",
                "Hit2",
                "Hit3",
                "Hit4",
                "Hit5",
                "Hit6",
                "Hit7",
                "Hit8",
                "Hit9",
            };
            AudioManager.instance.PlayRandomAudio(transform, names, 0.5f, Random.Range(0.98f, 1.02f));
            yield return new WaitForSeconds(hitAudioCouldown);
            hitCoroutine = null;
        }

        public void GetSpitted()
        {
            spitAnim.SetBool("Spitted", true);
            spit.gameObject.SetActive(true);
            spit.Init(this);
            isSpitted = true;
            canMove = false;
        }

        public void UnSpitted()
        {
            canMove = true;
            isSpitted = false;
            spitAnim.SetBool("Spitted", false);
        }

        public void Die()
        {
            AudioManager.instance.PlayAudio(transform, "GameOver", 0.3f);
            crystalOnDeathTxt.text = FragmentTransfert.instance.GetFragmentsSaved().ToString();
            if (tuto) waveOnDeathTxt.text = "Tutorial";
            else waveOnDeathTxt.text = (WaveManager.instance.currentWave - 1).ToString();
            System.DateTime dieTime = System.DateTime.Now;
            float gameDuration = Time.time - startTime;

            //calcul minutes left
            int minutes = (int)(gameDuration / 60);
            int secondes = (int)(gameDuration - (minutes * 60));

            // write time in XX:XX format
            if (minutes < 10)
            {
                if (secondes < 10) timeOnDeathTxt.text = new string("0" + minutes + ":0" + secondes);
                else timeOnDeathTxt.text = new string("0" + minutes + ":" + secondes);
            }
            else
            {
                if (secondes < 10) timeOnDeathTxt.text = new string(minutes + ":0" + secondes);
                else timeOnDeathTxt.text = new string(minutes + ":" + secondes);
            }

            canMove = false;
            canRotate = false;
            anim.SetTrigger("Die");
            StartCoroutine(WaitDeathAnim());
        }

        private IEnumerator WaitDeathAnim()
        {
            yield return new WaitForSeconds(deathAnimationDuration);
            SceneManager.LoadScene(0);
        }

        public void AddHealth(float _health)
        {
            getHealed.Invoke();
            currentHealth += _health;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            UpdateHealthBar();
        }

        public void SetHealth(float _health)
        {
            currentHealth = _health;
            UpdateHealthBar();
        }

        public float GetHealth()
        {
            return currentHealth;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        private void UpdateHealthBar()
        {
            healthBar.value = currentHealth / maxHealth;
        }

        #endregion

        #region OOP Gestion
        public void OutOfPower()
        {
            healthBarAnim.SetBool("PowerOn", false);
            if (!isAiming)
            {
                crosshairAnim.SetTrigger("PowerOff");
            }
            crosshairAnim.SetBool("Power", false);
            notEnoughtCrystal.SetTrigger("OOP");
            notEnoughtCrystal.SetBool("PowerOn", false);
        }

        public void PowerBack()
        {
            healthBarAnim.SetBool("PowerOn", true);
            if (!isAiming)
            {
                crosshairAnim.SetTrigger("PowerBack");
            }
            crosshairAnim.SetBool("Power", true);
            notEnoughtCrystal.SetTrigger("PowerBack");
            notEnoughtCrystal.SetBool("PowerOn", true);
        }
        #endregion

        #region Trigger

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Center")) isInCenter = true;
            if ((other.CompareTag("DeathArea"))) TakeDamage(9999);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Center")) isInCenter = false;
        }
        #endregion

        #region Sub events
        public void SubGetCrystal(UnityAction action)
        {
            getCrystal.AddListener(action);
        }
        public void UnsubGetCrystal(UnityAction action)
        {
            getCrystal.RemoveListener(action);
        }

        public void SubGetHealed(UnityAction action)
        {
            getHealed.AddListener(action);
        }
        public void UnsubGetHealed(UnityAction action)
        {
            getHealed.RemoveListener(action);
        }
        public void SubSaveCrystal(UnityAction action)
        {
            saveCrystal.AddListener(action);
        }
        public void UnsubSaveCrystal(UnityAction action)
        {
            saveCrystal.RemoveListener(action);
        }

        #endregion

        #region Getter
        public InventorySystem GetInventory() { return inventory; }

        #endregion
    }
}
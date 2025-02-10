using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

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
		[Range(0,10)]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		[Range(0, 20)]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		[Range(0, 5)]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		private bool canMove = true;
		private bool canRotate = true;
		[SerializeField]
		private PlayerInput playerInput;

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



		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;


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

		[Header("HEALTH")]
		[SerializeField]
		private float maxHealth = 100;
		[SerializeField]
		private float currentHealth;

		[Header("Inventory")]
		[SerializeField]
		private InventorySystem inventory;
		[SerializeField]
		private GameObject turretSniper;

		[Header("Interaction")]
		[SerializeField]
		private float interactionDistance = 5;
		[SerializeField]
		private float minningStrenght;
		private bool interacting;
		private bool minning;
		private bool crafting;
		FragmentBehaviour fragment;
		DesktopType desktop;
		[SerializeField]
		private InputAction interactAction;
		private bool turretInHand;

		[Header("UI")]
		[SerializeField]
		private GameObject notEnoughtMessage;

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
			bulletLeftInMagazine = maxBulletInMagazine;
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
			timeSinceLastBullet = shotCouldown;
			currentHealth = maxHealth;
			playerInput.actions["Interact"].canceled += StopInteract;
		}

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			if(canMove)Move();

			if(canRotate) Aim();
			if (interacting && minning) { Minning(); }
			if (interacting && crafting) { Crafting(); }


			else
			{
				interacting = false;
				minning = false;
				canMove = true;
				canRotate = true;
			}
			timeSinceLastBullet += Time.deltaTime;

			RaycastHit hit;

			if (Physics.Raycast(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward), out hit, interactionDistance, InteractLayers))
			{
				if(hit.collider.CompareTag("generator"))
                {
					if(inventory.GetFragmentQuantity() <= 0)
                    {
						notEnoughtMessage.SetActive(true);
					}
					else notEnoughtMessage.SetActive(false);
				}
				else notEnoughtMessage.SetActive(false);
			}
			else notEnoughtMessage.SetActive(false);


			#region Reload Timer
			if (bulletLeftInMagazine <= 0)
			{

			}
			#endregion

			if (inventory.GetItemInHand().CompareTag("turret"))
			{
				RaycastHit hit2;
				if (Physics.Raycast(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward), out hit2, InteractLayers))
				{
					turretSniper.transform.position = hit2.transform.position;
				}
			}


		}

        private void LateUpdate()
		{
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

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
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
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


        #region Weapon

		private void Aim()

        {
			if(isAiming != _input.aim && inventory.GetItemInHand().name == "Gun")
            {
				isAiming = _input.aim;
				weaponAnim.SetBool("Aim", isAiming);
			}
        }

		private void OnShot()
		{
			if(timeSinceLastBullet>= shotCouldown && inventory.GetItemInHand().name == "Gun")
            {
				if (bulletLeftInMagazine > 0)
				{
					BulletBehaviour bullet = poolBullets.GetBullet();
					weaponAnim.SetTrigger("Shot");
					timeSinceLastBullet = 0;
					bullet.Launch();
					bulletLeftInMagazine--;
					Debug.Log(bulletLeftInMagazine);
					if (bulletLeftInMagazine <= 0)
					{
                        StartCoroutine(WaitReloadTimer());
					}
				}
            }
		}


		private void OnReload()
		{
			if(bulletLeftInMagazine< maxBulletInMagazine)
			{
				bulletLeftInMagazine = 0;
                StartCoroutine(WaitReloadTimer());
            }
        }

		private IEnumerator WaitReloadTimer()
		{
            yield return new WaitForSeconds(shotCouldown);
            weaponAnim.SetTrigger("Reload");
            yield return new WaitForSeconds(reloadTime);
            bulletLeftInMagazine = maxBulletInMagazine;
		}

        #endregion



		private void OnScroll()
        {
			inventory.Scroll(_input.newWeapon);
        }


        #region Interaction


        private void OnInteract()
        {
			RaycastHit hit;
			interacting = true;
			Debug.DrawRay(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward) * interactionDistance, Color.green, 100);
			
			if (Physics.Raycast(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.TransformDirection(Vector3.forward) , out hit, interactionDistance, InteractLayers))
            {
				Debug.Log(hit + " | " + hit.collider.gameObject);
				switch(hit.collider.tag)
				{
					case "generator":
						GeneratorBehaviour generator = hit.collider.gameObject.GetComponent<GeneratorBehaviour>();
                        if (generator != null)
                        {
                            if (inventory.RemoveFragment(1))
                            {
                                generator.AddFragment(1);
                            }
                        }
                        break;

                    case "desktop":
						desktop = hit.collider.gameObject.GetComponentInParent<DesktopType>();
						crafting = true;
						canMove = false;
						canRotate = false;
						break;

                    case "turret":

                        break;

					case "fragment":
						fragment = hit.collider.gameObject.GetComponentInParent<FragmentBehaviour>();
						minning = true;
						canMove = false;
						canRotate = false;
						break;

                    default:
						break;
				}
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
			if(fragment == null)
            {
				minning = false;
				return;
            }
			fragment.Hit(minningStrenght);
			if (fragment.GetHealth() <= 0)
            {
				//Gain crystal
				inventory.AddFragment(fragment.GetQuantity());
			}
		}

		private void Crafting()
		{
			if(desktop == null)
            {
				crafting = false;
				return;
            }
			if(desktop.Craft(inventory))
            {
				switch(desktop.turretType)
                {
					case "Sniper":
						inventory.AddItem(turretSniper);
						break;

					case "Mitraillette":
						inventory.AddItem(turretSniper);
						break;

					default:
						Debug.Log("Turret name not found");
						break;
                }
				crafting = false;
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

        public void TakeDamage(float damage)
        {
			currentHealth -= damage;
			if (currentHealth <= 0)
			{
				Die();
			}
		}

        public void Die()
        {
            //throw new System.NotImplementedException();
        }

		public void SetHealth(float _health)
		{
			currentHealth = _health;
		}

		public float GetHealth()
		{
			return currentHealth;
		}

        public float GetMaxHealth()
        {
            return maxHealth;
        }
    }
}
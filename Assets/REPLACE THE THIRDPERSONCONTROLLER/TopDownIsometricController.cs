using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using UnityEngine.InputSystem;

public class TopDownIsometricController : NetworkBehaviour
{
    #region Variables

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float turnSmoothTime = 0.2f;
    public float jumpForce = 10f;
    public float gravity = -9.81f;
    public float swimSpeed = 5f;
    public float flySpeed = 10f;
    public float waterSurfaceLevel = 0.0f;
    private PlayerData playerData;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public LayerMask groundMask;
    public LayerMask waterMask;
    public Camera distantPlayerCamera;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera followCamera;
    public CinemachineVirtualCamera orbitalCamera;
    public CinemachineBrain cinemachineBrain;
    public CinemachineInputProvider inputProvider;

    [Header("Character Components")]
    public CharacterController characterController;
    private Animator animator;
    public AudioListener audioListener;

    [Header("Sound Settings")]
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip swimSound;
    public AudioClip flySound;
    public AudioClip pvpActivateSound;
    public AudioClip pvpDeactivateSound;
    private AudioSource audioSource;
    private Coroutine walkSoundCoroutine;

    [Header("PvP Settings")]
    public bool isPvPEnabled = false;
    public KeyCode togglePvPKey = KeyCode.Comma;
    public Animator pvpAnimator;
    private float pvpCooldown = 5f;
    private float lastPvPToggleTime = -5f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isFlying = false;
    private bool isSwimming = false;
    private bool isSprinting = false;
    private bool isJumping = false;

    private static List<ThirdPersonController> players = new List<ThirdPersonController>();

    #endregion

    #region Initialization

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        InitializeComponents();
        EnableLocalPlayerComponents();
        Debug.Log("Local player started");
        SetupCinemachine();
    }
    private void SetupCinemachine()
    {
        if (followCamera != null)
        {
            // Associer la caméra Cinemachine au joueur local
            followCamera.Follow = transform;
            followCamera.LookAt = transform;
        }

        if (orbitalCamera != null)
        {
            // Si vous utilisez également une caméra orbitale, vous pouvez l'associer de la même manière
            orbitalCamera.Follow = transform;
            orbitalCamera.LookAt = transform;
        }
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isLocalPlayer)
        {
            DisableLocalPlayerComponents();
            Debug.Log("This is not the local player");
        }
    }

    private void InitializeComponents()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (!characterController) Debug.LogError("CharacterController component is missing!");
        if (!animator) Debug.LogError("Animator component is missing!");
        if (!audioSource) Debug.LogError("AudioSource component is missing!");
    }

    private void EnableLocalPlayerComponents()
    {
        distantPlayerCamera?.gameObject.SetActive(true);
        EnableCinemachineComponents();
        characterController.enabled = true;
    }

    private void DisableLocalPlayerComponents()
    {
        distantPlayerCamera?.gameObject.SetActive(false);
        DisableCinemachineComponents();
        characterController.enabled = false;
    }

    private void EnableCinemachineComponents()
    {
        if (cinemachineBrain)
        {
            cinemachineBrain.enabled = true;
            ToggleCinemachineCameras(true);
        }

        if (inputProvider)
        {
            inputProvider.enabled = true;

            // Créez un InputAction et liez-le
            InputAction action = new InputAction(type: InputActionType.Value, binding: "<Mouse>/delta");

            // Convertissez en InputActionReference
            InputActionReference actionReference = InputActionReference.Create(action);

            inputProvider.XYAxis = actionReference;
        }

    }

    private void DisableCinemachineComponents()
    {
        if (cinemachineBrain)
        {
            cinemachineBrain.enabled = false;
            ToggleCinemachineCameras(false);
        }
        inputProvider.enabled = false;
    }

    private void ToggleCinemachineCameras(bool state)
    {
        foreach (var vc in cinemachineBrain.GetComponentsInChildren<CinemachineVirtualCamera>(true))
        {
            vc.enabled = state;
        }
    }

    #endregion

    #region Update Methods

    private void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
        HandleGrounding();
        HandleJump();
        HandleFly();
        HandleSwim();
        HandleSprint();
        HandleGravity();
        HandlePvP();
    }

    private void HandleMovement()
    {
        if (isFlying || isSwimming) return;

        // Check if the player is grounded
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensure the player sticks to the ground
        }

        // Get movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Apply movement based on input and sprinting state
        MoveCharacter(move);

        // Adjust the camera and character rotation during ground movement
        AdjustCameraAndCharacterRotation(move);

        // Apply gravity
        ApplyGravity();  // Use the method to apply gravity

        // Update animator state based on movement
        bool isMoving = move != Vector3.zero;
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            PlayWalkingSound();
        }
        else
        {
            StopWalkingSound();
        }
    }

    private void AdjustCameraAndCharacterRotation(Vector3 moveDirection)
    {
        // Rotation du personnage basé sur la direction du mouvement
        if (moveDirection != Vector3.zero)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

            // Rotation horizontale du personnage
            transform.Rotate(Vector3.up * mouseX);

            // Ajuster l'angle de la caméra en fonction du mouvement vertical de la souris
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            float newCameraRotationX = followCamera.transform.localEulerAngles.x - mouseY;
            newCameraRotationX = Mathf.Clamp(newCameraRotationX, -60f, 60f);
            followCamera.transform.localEulerAngles = new Vector3(newCameraRotationX, followCamera.transform.localEulerAngles.y, 0);
        }
    }

    private void MoveCharacter(Vector3 direction)
    {
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        characterController.Move(direction * currentSpeed * Time.deltaTime);
        animator.SetFloat("speed", currentSpeed);
    }

    private void HandleJump()
    {
        if (isSwimming) return;

        if (Input.GetButtonDown("Jump") && isGrounded && !isFlying && !isJumping)
        {
            Jump();
        }

        ApplyGravity();
    }
    public PlayerData GetPlayerData()
    {
        return playerData;
    }
    private void Jump()
    {
        animator.SetTrigger("Jump");
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        isJumping = true;
        audioSource.PlayOneShot(jumpSound);
        StartCoroutine(ResetJumpFlag());
    }

    private IEnumerator ResetJumpFlag()
    {
        yield return new WaitForSeconds(0.1f);
        isJumping = false;
    }

    private void HandleGrounding()
    {
        isGrounded = characterController.isGrounded;
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
            animator.ResetTrigger("Jump");
        }
    }

    private void HandleFly()
    {
        if (isSwimming) return;

        if (Input.GetButtonDown("Fly"))
        {
            ToggleFlying();
        }

        if (isFlying)
        {
            FlyMovement();
        }
    }

    private void ToggleFlying()
    {
        isFlying = !isFlying;
        if (isFlying)
        {
            StartFlying();
        }
        else
        {
            StopFlying();
        }
    }

    private void StartFlying()
    {
        animator.SetBool("isFlying", true);
        velocity.y = 0;
        PlaySound(flySound);
    }

    private void StopFlying()
    {
        animator.SetTrigger("startFall");
        velocity.y = 0;
        animator.SetBool("isFlying", false);
        StopSound();
    }

    private void FlyMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Move up or down based on input
        float moveY = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            moveY = 0.56f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveY = -0.56f;
        }

        // Calculate the move direction based on input
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ + transform.up * moveY;
        characterController.Move(moveDirection * flySpeed * Time.deltaTime);

        // Adjust the camera and character rotation during flight
        AdjustCameraDuringFlight();
    }

    private void AdjustCameraDuringFlight()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation horizontale du personnage
        transform.Rotate(Vector3.up * mouseX);

        // Ajuster l'angle de la caméra en fonction du mouvement vertical de la souris
        float newCameraRotationX = followCamera.transform.localEulerAngles.x - mouseY;
        newCameraRotationX = Mathf.Clamp(newCameraRotationX, -60f, 60f);
        followCamera.transform.localEulerAngles = new Vector3(newCameraRotationX, followCamera.transform.localEulerAngles.y, 0);
    }

    private void ApplyGravity()
    {
        // Check if the method should apply gravity: skip if not the local player, or if the character is flying or swimming.
        if (!isLocalPlayer || isFlying || isSwimming) return;

        // Increase the downward velocity over time to simulate gravity. The gravity value is negative.
        velocity.y += gravity * Time.deltaTime;

        // Move the character controller downwards based on the updated velocity.
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleSwim()
    {
        bool isInWater = Physics.CheckSphere(transform.position, 0.5f, waterMask);

        if (isInWater)
        {
            if (!isSwimming)
            {
                StartSwimming();
            }
            SwimMovement();
        }
        else if (isSwimming)
        {
            StopSwimming();
        }
    }

    private void StartSwimming()
    {
        isSwimming = true;
        animator.SetBool("isSwimming", true);
        PlaySound(swimSound);
    }

    private void StopSwimming()
    {
        isSwimming = false;
        animator.SetBool("isSwimming", false);
        StopSound();
    }

    private void SwimMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        if (transform.position.y < waterSurfaceLevel)
        {
            moveDirection.y = Input.GetKey(KeyCode.Space) ? swimSpeed : Input.GetKey(KeyCode.LeftControl) ? -swimSpeed : 0;
        }
        else
        {
            moveDirection.y = -swimSpeed * 0.5f;
        }

        characterController.Move(moveDirection * swimSpeed * Time.deltaTime);
        animator.SetBool("isSwimming", moveDirection != Vector3.zero);
    }

    private void HandleSprint()
    {
        if (isSwimming) return;

        isSprinting = Input.GetButton("Sprint");
    }
    private void HandleGravity()
    {
        if (isFlying || isSwimming) return;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
  

    private void HandlePvP()
    {
        if (Time.time - lastPvPToggleTime < pvpCooldown) return;

        if (Input.GetKeyDown(togglePvPKey))
        {
            TogglePvP();
        }
    }

    private void TogglePvP()
    {
        isPvPEnabled = !isPvPEnabled;
        lastPvPToggleTime = Time.time;

        pvpAnimator?.SetTrigger(isPvPEnabled ? "PvPEnabled" : "PvPDisabled");

        GetComponent<Renderer>().material.color = isPvPEnabled ? Color.red : Color.white;
        PlaySound(isPvPEnabled ? pvpActivateSound : pvpDeactivateSound);

        if (isServer)
        {
            RpcNotifyPvPStatus(isPvPEnabled);
        }

        ShowPvPStatusUI(isPvPEnabled);
    }

    private void ShowPvPStatusUI(bool isPvPEnabled)
    {
        string status = isPvPEnabled ? "PvP Enabled" : "PvP Disabled";
        UIManager.Instance.ShowNotification(status);
    }

    [ClientRpc]
    private void RpcNotifyPvPStatus(bool isPvPEnabled)
    {
        string playerName = GetComponent<Player>().playerName;
        string status = isPvPEnabled ? "enabled PvP" : "disabled PvP";
        UIManager.Instance.ShowGlobalNotification($"{playerName} has {status}!");
    }

    #endregion

    #region Sound Management

    private void PlayWalkingSound()
    {
        if (!audioSource.isPlaying || audioSource.clip != walkSound)
        {
            PlaySound(walkSound);
        }
    }

    private void StopWalkingSound()
    {
        if (audioSource.isPlaying && audioSource.clip == walkSound)
        {
            StopSound();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void StopSound()
    {
        audioSource.Stop();
    }

    #endregion

    #region Cleanup

    private void OnApplicationQuit()
    {
        if (isServer)
        {
            CleanupPlayer();
        }
    }

    private void CleanupPlayer()
    {
        RemovePlayer(this);
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void RemovePlayer(ThirdPersonController player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isServer)
        {
            CleanupPlayer();
        }
    }

    #endregion
    void Awake()
    {
        // Initialize the PlayerData with some values
        playerData = new PlayerData()
        {
            Mana = 100,
            Strength = 20,
            Agility = 15,
            Intelligence = 25
        };
    }
}

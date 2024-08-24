using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
/*
 * Script Explanation: ThirdPersonController
 *
 * Overview:
 * This script controls the movement, animations, and sound effects of a third-person character in a multiplayer environment using the Mirror networking library. It handles various movement states such as walking, sprinting, jumping, flying, and swimming, ensuring that the appropriate animations and sounds are played based on the player's actions.
 *
 * Key Sections:
 *
 * 1. Variables and Components:
 *    - The script defines several public variables for movement settings, camera settings, sound settings, and PvP settings.
 *    - The main components include `CharacterController` for handling movement, `Animator` for controlling animations, and `AudioSource` for playing sound effects.
 *
 * 2. Initialization:
 *    - `OnStartLocalPlayer()` and `OnStartClient()` methods initialize the character's components and set up the player as the local player or a remote player.
 *    - The `EnableLocalPlayerComponents()` and `DisableLocalPlayerComponents()` methods enable or disable components like cameras and input providers based on whether the player is the local player or not.
 *
 * 3. Movement Handling:
 *    - `HandleMovement()`: Manages basic movement (walking and sprinting). It calculates the movement direction based on player input and updates the character's rotation and position accordingly. It also controls the walking sound, ensuring it plays only when the player is moving.
 *    - `HandleSprint()`: Adjusts the character's speed based on whether the player is sprinting or walking.
 *    - `HandleJump()`: Handles jumping, applying an upward force to the character's velocity and playing a jump sound.
 *    - `HandleFly()`: Toggles flying mode, allowing the character to move freely in 3D space. The fly sound is played when the player starts flying and stopped when they land.
 *    - `HandleSwim()`: Detects if the player is in water and manages swimming movement. The swim sound is played while the player is swimming.
 *    - `HandleGravity()`: Applies gravity to the character's movement when they are not flying or swimming.
 *
 * 4. Ground Detection:
 *    - `Ground()`: Checks if the player is on the ground using the `CharacterController.isGrounded` property. It resets the player's velocity when grounded and stops the jump animation.
 *
 * 5. Sound Management:
 *    - The script plays different sounds based on the player's movement state. It uses the `AudioSource` component to play, stop, and switch between sounds such as walking, swimming, flying, and jumping.
 *
 * 6. PvP (Player vs Player) Mode:
 *    - `HandlePvP()`: Toggles PvP mode when the specified key is pressed. The PvP state is reflected in an animator parameter, allowing the game to visually represent the player's PvP status.
 *
 * 7. Network Cleanup:
 *    - `OnApplicationQuit()`, `CleanupPlayer()`, and `OnStopClient()` ensure that the player's networked object is properly removed when they disconnect or quit the game.
 *
 * 8. Coroutine Management:
 *    - The script uses coroutines, such as in `PlayWalkSound()`, to manage repeated actions over time, like playing the walking sound at intervals.
 *
 * Summary:
 * This script is designed to control a third-person character in a multiplayer game environment, handling complex movement states and their corresponding animations and sound effects. It also includes networking features to properly manage player instances across clients.
 */
public class ThirdPersonController : NetworkBehaviour
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
        inputProvider.enabled = true;
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
    /// <summary>
    /// Applies gravity to the character when not flying or swimming.
    /// This method continuously increases the downward velocity of the character,
    /// simulating the effect of gravity. It also moves the character downwards based on the calculated velocity.
    /// </summary>
    private void ApplyGravity()
    {
        // Check if the method should apply gravity: skip if not the local player, or if the character is flying or swimming.
        if (!isLocalPlayer || isFlying || isSwimming) return;

        // Increase the downward velocity over time to simulate gravity. The gravity value is negative.
        velocity.y += gravity * Time.deltaTime;

        // Move the character controller downwards based on the updated velocity.
        characterController.Move(velocity * Time.deltaTime);
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
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

        float moveY = Input.GetKey(KeyCode.Space) ? 1 : Input.GetKey(KeyCode.LeftControl) ? -1 : 0;

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ + Vector3.up * moveY;
        characterController.Move(moveDirection * flySpeed * Time.deltaTime);

        AdjustCameraDuringFlight();
    }

    private void AdjustCameraDuringFlight()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.Rotate(Vector3.right * -mouseY);

        Vector3 currentEulerAngles = Camera.main.transform.localEulerAngles;
        currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, -60f, 60f);
        Camera.main.transform.localEulerAngles = currentEulerAngles;
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
}

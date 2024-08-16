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
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float turnSmoothTime = 0.2f;
    public float turnSmoothVelocity = 0.2f;
    public float jumpForce = 10f;
    public float gravity = -9.81f;
    public float swimSpeed = 5f;
    public float flySpeed = 10f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isFlying = false;
    private bool isSwimming = false;
    private bool isSprinting = false;
    private bool isJumping = false;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;

    public LayerMask groundMask;
    public LayerMask waterMask;

    public Camera distantPlayerCamera;
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
    private AudioSource audioSource;
    private Coroutine walkSoundCoroutine;

    [Header("PvP Settings")]
    public bool isPvPEnabled = false;
    public KeyCode togglePvPKey = KeyCode.Comma;
    public Animator pvpAnimator;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        UnityEngine.Debug.Log("Local player started");

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (characterController == null)
        {
            UnityEngine.Debug.LogError("CharacterController component is missing!");
        }
        if (animator == null)
        {
            UnityEngine.Debug.LogError("Animator component is missing!");
        }
        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource component is missing!");
        }

        EnableLocalPlayerComponents();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        UnityEngine.Debug.Log("Client started");

        if (!isLocalPlayer)
        {
            UnityEngine.Debug.Log("This is not the local player");
            DisableLocalPlayerComponents();
        }
    }

    private void DisableLocalPlayerComponents()
    {
        if (distantPlayerCamera != null)
        {
            distantPlayerCamera.gameObject.SetActive(false);
        }

        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled = false;

            CinemachineVirtualCamera[] virtualCameras = cinemachineBrain.GetComponentsInChildren<CinemachineVirtualCamera>(true);
            foreach (CinemachineVirtualCamera vc in virtualCameras)
            {
                vc.enabled = false;
            }
        }


        if (inputProvider != null)
        {
            inputProvider.enabled = false;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }
    }

    private void EnableLocalPlayerComponents()
    {
        if (distantPlayerCamera != null)
        {
            distantPlayerCamera.gameObject.SetActive(true);
        }

        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled = true;

            CinemachineVirtualCamera[] virtualCameras = cinemachineBrain.GetComponentsInChildren<CinemachineVirtualCamera>(true);
            foreach (CinemachineVirtualCamera vc in virtualCameras)
            {
                vc.enabled = true;
            }
        }


        if (inputProvider != null)
        {
            inputProvider.enabled = true;
        }

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    void OnApplicationQuit()
    {
        if (isServer)
        {
            CleanupPlayer();
        }
    }

    void CleanupPlayer()
    {
        RemovePlayer(this);
        NetworkServer.Destroy(gameObject);
    }

    private static List<ThirdPersonController> players = new List<ThirdPersonController>();

    [Server]
    void RemovePlayer(ThirdPersonController player)
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

    private void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
        Ground();
        HandleJump();
        HandleFly();
        HandleSwim();
        HandleSprint();
        HandleGravity();
        HandlePvP();
    }
    private void HandleMovement()
    {
        if (!isLocalPlayer || isFlying || isSwimming) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

            animator.SetFloat("speed", currentSpeed);
        }
        else
        {
            animator.SetFloat("speed", 0);
        }

        bool isMoving = move != Vector3.zero;
        animator.SetBool("isWalking", isMoving);

        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.clip = walkSound;
            audioSource.Play();
        }
        else if (!isMoving && audioSource.isPlaying && audioSource.clip == walkSound)
        {
            audioSource.Stop();
        }
    }
    private IEnumerator PlayWalkSound()
    {
        while (true)
        {
            audioSource.PlayOneShot(walkSound);
            yield return new WaitForSeconds(0.5f); // Adjust the timing as needed
        }
    }
    private void HandleJump()
    {
        if (!isLocalPlayer || isSwimming) return;

        if (Input.GetButtonDown("Jump") && isGrounded && !isFlying && !isJumping)
        {
            animator.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            audioSource.PlayOneShot(jumpSound);
        }
    }
    private void Ground()
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
        if (!isLocalPlayer || isSwimming) return;

        if (Input.GetButtonDown("Fly"))
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

        if (isFlying)
        {
            FlyMovement();
        }
    }

    private void StartFlying()
    {
        animator.SetBool("isFlying", true);
        velocity.y = 0;
        audioSource.Stop(); // Stop any current sound
        audioSource.clip = flySound;
        audioSource.Play();
    }

    private void StopFlying()
    {
        animator.SetTrigger("startFall");
        velocity.y = 0;
        animator.SetBool("isFlying", false);
        audioSource.Stop(); // Stop the fly sound
    }

    private void FlyMovement()
    {
        if (!isLocalPlayer) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0;

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        // Utilisation de la touche pour monter et descendre
        if (Input.GetKey(KeyCode.Space))
        {
            moveY = 1;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveY = -1;
        }

        Vector3 move = moveDirection * flySpeed + Vector3.up * moveY * flySpeed;
        characterController.Move(move * Time.deltaTime);

        // Rotation de la caméra avec la souris
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleSwim()
    {
        if (!isLocalPlayer) return;

        bool isInWater = Physics.CheckSphere(transform.position, 0.5f, waterMask);

        if (isInWater)
        {
            if (!isSwimming)
            {
                StartSwimming();
            }
            SwimMovement();
        }
        else
        {
            if (isSwimming)
            {
                StopSwimming();
            }
        }
    }

    private void StartSwimming()
    {
        isSwimming = true;
        animator.SetBool("isSwimming", true);
        audioSource.Stop(); // Stop any current sound
        audioSource.clip = swimSound;
        audioSource.Play();
    }

    private void StopSwimming()
    {
        isSwimming = false;
        animator.SetBool("isSwimming", false);
        audioSource.Stop(); // Stop the swim sound
    }

    private void SwimMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
   

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

       

        Vector3 move = moveDirection * swimSpeed + Vector3.up * swimSpeed;
        characterController.Move(move * Time.deltaTime);

        bool isMoving = move != Vector3.zero;
        animator.SetBool("isSwimming", isMoving);
    }

    private void HandleSprint()
    {
        if (!isLocalPlayer || isSwimming) return;

        isSprinting = Input.GetButton("Sprint");

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        animator.SetFloat("speed", move.magnitude * currentSpeed);
    }

    private void HandleGravity()
    {
        if (!isLocalPlayer || isFlying || isSwimming) return;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandlePvP()
    {
        if (Input.GetKeyDown(togglePvPKey))
        {
            isPvPEnabled = !isPvPEnabled;
            if (pvpAnimator != null)
            {
                pvpAnimator.SetBool("isPvPEnabled", isPvPEnabled);
            }
        }
    }
}

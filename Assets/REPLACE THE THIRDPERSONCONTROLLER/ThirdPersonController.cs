using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class ThirdPersonController : NetworkBehaviour
{

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 10f;
    public float gravity = -9.81f;
    public float flySpeed = 10f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isFlying = false;
    private bool isSprinting = false;
    private bool isJumping = false;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;

    public LayerMask groundMask;
    public LayerMask collisionMask;
    private Collider waterCollider;
    public Camera distantPlayerCamera;
    public CinemachineBrain cinemachineBrain;
    public CinemachineInputProvider inputProvider;
    public CinemachineFreeLook cinemachineFreeLook;

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

    [Header("Swimming Settings")]
    public float swimSpeed = 4f;
    public LayerMask waterMask;

    private bool isSwimming = false;

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

        if (cinemachineFreeLook != null)
        {
            cinemachineFreeLook.gameObject.SetActive(false);
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

        if (cinemachineFreeLook != null)
        {
            cinemachineFreeLook.gameObject.SetActive(true);
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

        CheckIfSwimming();
        HandleMovement();
        Ground();
        HandleJump();
        HandleFly();
        HandleSwimming();
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
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        bool isMoving = move != Vector3.zero;
        animator.SetBool("isWalking", isMoving);

        if (isMoving && walkSoundCoroutine == null)
        {
            walkSoundCoroutine = StartCoroutine(PlayWalkSound());
        }
        else if (!isMoving && walkSoundCoroutine != null)
        {
            StopCoroutine(walkSoundCoroutine);
            walkSoundCoroutine = null;
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

    private void HandleSwimming()
    {
        if (isSwimming)
        {
            SwimMovement();
        }
    }

    private void CheckIfSwimming()
    {
        bool wasSwimming = isSwimming;

        isSwimming = Physics.Raycast(transform.position, Vector3.down, 1f, waterMask);

        if (isSwimming && !wasSwimming)
        {
            StartSwimming();
        }
        else if (!isSwimming && wasSwimming)
        {
            StopSwimming();
        }
    }

    private void SwimMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * swimSpeed * Time.deltaTime);
    }


    private void StartSwimming()
    {
        isSwimming = true;
        animator.SetBool("isSwimming", true);
        velocity.y = 0;
        audioSource.PlayOneShot(swimSound);
    }

    private void StopSwimming()
    {
        isSwimming = false;
        animator.SetBool("isSwimming", false);
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
        if (!isLocalPlayer) return;

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
        else if (!isSwimming)
        {
            HandleMovement();
        }
    }

    private void StartFlying()
    {
        animator.SetBool("isFlying", true);
        velocity.y = 0;
        audioSource.PlayOneShot(flySound);
    }

    private void StopFlying()
    {
        animator.SetTrigger("startFall");
        velocity.y = 0;
        animator.SetBool("isFlying", false);
    }

    private void FlyMovement()
    {
        if (!isLocalPlayer) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0;

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        // Utilisation de la souris pour monter et descendre
        float mouseScroll = Input.GetAxis("Mouse Y");
        if (mouseScroll > 0)
        {
            moveY = 1;
        }
        else if (mouseScroll < 0)
        {
            moveY = -1;
        }

        Vector3 move = moveDirection * flySpeed + Vector3.up * moveY * flySpeed;
        characterController.Move(move * Time.deltaTime);

        // Rotation de la caméra avec la souris
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
       
      
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

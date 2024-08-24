using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mirror;

public class SkeletonAI : NetworkBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float idleTime = 2f;
    public LayerMask obstacleLayer;
    public float obstacleDetectionDistance = 1f;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float chaseRange = 5f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;

    [Header("Respawn Settings")]
    public float respawnTime = 5f;
    public Vector3 respawnPoint;

    [Header("Ground Check Settings")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Gravity Settings")]
    public float gravity = 9.81f;
    private Vector3 velocity;

    [Header("Audio Settings")]
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioClip footstepSound;
    public AudioClip combatMusic;
    public AudioClip patrolMusic;
    private AudioSource audioSource;
    private AudioSource musicSource;

    [Header("UI Settings")]
    public Slider healthBarPrefab;

    private int currentWaypointIndex = 0;
    private Transform currentPlayerTarget;
    private Animator animator;
    private bool isDead = false;
    private bool isGrounded = false;
    private bool isIdle = false;
    private bool canAttack = true;
    private Transform modelTransform;
    private Camera mainCamera;
    private Transform playerTransform;

    private ObjectiveManager objectiveManager;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        modelTransform = transform;
        audioSource = GetComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.clip = patrolMusic;
        musicSource.Play();

        mainCamera = Camera.main;

        objectiveManager = FindObjectOfType<ObjectiveManager>();
    }

    void Update()
    {
        if (isDead) return;

        CheckGroundStatus();

        currentPlayerTarget = GetClosestPlayer();

        if (currentPlayerTarget == null)
        {
            Patrol();
            if (musicSource.clip != patrolMusic)
            {
                StartCoroutine(FadeOutMusic());
            }
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, currentPlayerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
            else if (distanceToPlayer <= chaseRange)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
                if (musicSource.clip != patrolMusic)
                {
                    StartCoroutine(FadeOutMusic());
                }
            }
        }

        ApplyGravity();
    }

    void CheckGroundStatus()
    {
        if (groundCheck == null)
        {
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (!isGrounded || isIdle) return;

        animator.SetBool("isMoving", true);

        if (!audioSource.isPlaying && footstepSound != null)
        {
            audioSource.PlayOneShot(footstepSound);
        }

        if (IsObstacleDetected())
        {
            StartCoroutine(IdleAndTurnAround());
        }
        else
        {
            MoveTowardsTarget(waypoints[currentWaypointIndex].position);

            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.2f)
            {
                StartCoroutine(IdlePause());
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
        }
    }

    void ChasePlayer()
    {
        if (currentPlayerTarget == null || !isGrounded) return;

        StopCoroutine(IdlePause());
        isIdle = false;
        animator.SetBool("isMoving", true);
        MoveTowardsTarget(currentPlayerTarget.position);

        if (musicSource.clip != combatMusic)
        {
            StartCoroutine(FadeInMusic(combatMusic));
        }
    }

    void AttackPlayer()
    {
        if (currentPlayerTarget == null || !isGrounded) return;

        if (canAttack)
        {
            StopCoroutine(IdlePause());
            isIdle = false;
            animator.SetBool("isMoving", false);
            animator.SetTrigger("attack");
            DealDamageToPlayer();
            if (attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            StartCoroutine(AttackCooldown());
        }
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Keep the movement on the XZ plane
        Vector3 move = direction * moveSpeed * Time.deltaTime;

        // Apply movement on X and Z axis
        transform.position += new Vector3(move.x, 0, move.z);

        // Check the ground beneath the skeleton
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f, groundLayer))
        {
            // Adjust the position based on the height of the terrain
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        // Rotate the skeleton towards the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    IEnumerator IdlePause()
    {
        isIdle = true;
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(idleTime);
        isIdle = false;
    }

    IEnumerator IdleAndTurnAround()
    {
        isIdle = true;
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(idleTime);
        isIdle = false;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    bool IsObstacleDetected()
    {
        return Physics.Raycast(transform.position, transform.forward, obstacleDetectionDistance, obstacleLayer);
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }
    }

    void DealDamageToPlayer()
    {
        if (currentPlayerTarget == null) return;

        Health playerHealth = currentPlayerTarget.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage, gameObject);
        }
    }

    Transform GetClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }

        return closestPlayer;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator FadeInMusic(AudioClip newClip)
    {
        if (musicSource.isPlaying)
        {
            for (float volume = 1f; volume >= 0; volume -= Time.deltaTime)
            {
                musicSource.volume = volume;
                yield return null;
            }
            musicSource.Stop();
        }

        musicSource.clip = newClip;
        musicSource.Play();

        for (float volume = 0; volume <= 1f; volume += Time.deltaTime)
        {
            musicSource.volume = volume;
            yield return null;
        }
    }

    IEnumerator FadeOutMusic()
    {
        if (musicSource.isPlaying)
        {
            for (float volume = 1f; volume >= 0; volume -= Time.deltaTime)
            {
                musicSource.volume = volume;
                yield return null;
            }
            musicSource.Stop();
            musicSource.clip = patrolMusic;
            musicSource.Play();

            for (float volume = 0; volume <= 1f; volume += Time.deltaTime)
            {
                musicSource.volume = volume;
                yield return null;
            }
        }
    }
    public void RegisterPlayer(GameObject player)
    {
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}

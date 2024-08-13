using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;
using Mirror;

/* 
 * BossAI Script
 * Created by MK-BNJ
 * Modified by ChatGPT
 * 
 * Description:
 * This script controls the AI behavior of a boss enemy. It handles patrolling between waypoints, 
 * chasing the player within a certain range, attacking the player when in range, and respawning after death.
 * The script also includes ground checks, obstacle detection, and health management. Additionally, it manages 
 * sound effects and background music transitions based on the boss's actions.
 */
public class BossAI : NetworkBehaviour
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
    public float strafeDistance = 3f;
    public float strafeSpeed = 2f;
    public float dodgeCooldown = 5f;

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
    private bool canDodge = true;
    private NavMeshAgent navMeshAgent;
    private Transform modelTransform;
    private Camera mainCamera;
    private Transform playerTransform;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        modelTransform = transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.clip = patrolMusic;
        musicSource.Play();

        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
        }

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isDead) return;

        CheckGroundStatus();

        currentPlayerTarget = GetClosestPlayer();

        if (currentPlayerTarget == null)
        {
            Patrol();
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
            }
        }

        if (navMeshAgent.velocity.magnitude > 0.1f && isGrounded)
        {
            Vector3 direction = navMeshAgent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }
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

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(footstepSound);
        }

        if (IsObstacleDetected())
        {
            StartCoroutine(IdleAndTurnAround());
        }
        else
        {
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);

            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.2f)
            {
                StartCoroutine(IdlePause());
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
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

    void ChasePlayer()
    {
        if (currentPlayerTarget == null || !isGrounded) return;

        StopCoroutine(IdlePause());
        isIdle = false;
        animator.SetBool("isMoving", true);
        navMeshAgent.SetDestination(currentPlayerTarget.position);

        if (musicSource.clip != combatMusic)
        {
            musicSource.clip = combatMusic;
            musicSource.Play();
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
            int attackType = Random.Range(0, 5); // Randomly choose one of the five attacks
            switch (attackType)
            {
                case 0:
                    animator.SetTrigger("attack1");
                    Attack1();
                    break;
                case 1:
                    animator.SetTrigger("attack2");
                    Attack2();
                    break;
                case 2:
                    animator.SetTrigger("attack3");
                    Attack3();
                    break;
                case 3:
                    animator.SetTrigger("attack4");
                    Attack4();
                    break;
                case 4:
                    animator.SetTrigger("attack5");
                    Attack5();
                    break;
            }
            audioSource.PlayOneShot(attackSound);

            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void Attack1()
    {
        DealDamageToPlayer();
    }

    void Attack2()
    {
        DealDamageToPlayer();
    }

    void Attack3()
    {
        DealDamageToPlayer();
    }

    void Attack4()
    {
        DealDamageToPlayer();
    }

    void Attack5()
    {
        DealDamageToPlayer();
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

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
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

    public void RegisterPlayer(GameObject player)
    {
        if (player.transform == null) return;
        playerTransform = player.transform;
    }

    void Strafe()
    {
        if (currentPlayerTarget == null || !isGrounded) return;

        Vector3 strafeDirection = Vector3.right * (Random.Range(0, 2) * 2 - 1); // Randomly choose left or right
        Vector3 strafePosition = transform.position + strafeDirection * strafeDistance;

        if (!Physics.Raycast(transform.position, strafeDirection, strafeDistance, obstacleLayer))
        {
            navMeshAgent.SetDestination(strafePosition);
            StartCoroutine(StrafePause());
        }
    }

    IEnumerator StrafePause()
    {
        animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(strafeSpeed);
        animator.SetBool("isMoving", false);
    }

    void Dodge()
    {
        if (currentPlayerTarget == null || !isGrounded || !canDodge) return;

        Vector3 dodgeDirection = Vector3.back; // Dodge backward
        Vector3 dodgePosition = transform.position + dodgeDirection * strafeDistance;

        if (!Physics.Raycast(transform.position, dodgeDirection, strafeDistance, obstacleLayer))
        {
            navMeshAgent.SetDestination(dodgePosition);
            StartCoroutine(DodgeCooldown());
        }
    }

    IEnumerator DodgeCooldown()
    {
        canDodge = false;
        animator.SetTrigger("dodge");
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    public void PerformAction()
    {
        float actionProbability = Random.Range(0f, 1f);
        if (actionProbability < 0.2f)
        {
            Strafe();
        }
        else if (actionProbability < 0.4f)
        {
            Dodge();
        }
        else
        {
            AttackPlayer();
        }
    }
}

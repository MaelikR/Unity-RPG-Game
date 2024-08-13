using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Mirror;
using UnityEngine.UI;

public class FFAAI : NetworkBehaviour
{
    public Transform[] waypoints;
    public Transform[] taskWaypoints;
    public float moveSpeed = 2f;
    public float respawnTime = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public float idleTime = 2f;
    public LayerMask obstacleLayer;
    public float obstacleDetectionDistance = 1f;
    public float gravity = 9.81f;
    public int maxHealth = 100;
    public float patrolRange = 10f;
    public float fleeDistance = 3f;

    public int currentHealth;
    private int currentWaypointIndex = 0;
    private int currentTaskWaypointIndex = 0;
    private Transform currentPlayerTarget;
    private Animator animator;
    public bool isDead = false;
    private bool isGrounded = false;
    private bool isIdle = false;
    private bool performingTask = false;
    public Vector3 respawnPoint;
    private Vector3 velocity;
    private NavMeshAgent navMeshAgent;
    private Transform modelTransform;
    public Slider healthBarEnemy;
    public float attackRadius = 1f; // Portée de l'attaque
    public int attackDamage = 10;   // Dégâts infligés par l'attaque
    public float attackInterval = 1f; // Intervalle entre les attaques
    private float lastAttackTime = 0f; // Temps du dernier attaque
    private bool hasBeenAttacked = false;

    public float chaseRange = 5f;  // Distance à partir de laquelle le PNJ commence à poursuivre le joueur
    public float chaseSpeed = 3.5f;

    // Liste des composants Renderer pour rendre le PNJ invisible
    private Renderer[] renderers;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        modelTransform = transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
        }
        respawnPoint = transform.position;

        // Initialisez la liste des composants Renderer
        renderers = GetComponentsInChildren<Renderer>();

        if (healthBarEnemy != null)
        {
            healthBarEnemy.maxValue = maxHealth;
            healthBarEnemy.value = currentHealth;
        }
    }

    void Update()
    {
        if (isDead) return;

        CheckGroundStatus();

        if (currentPlayerTarget == null)
        {
            FindPlayerTarget();
            if (currentPlayerTarget == null && !performingTask)
            {
                Patrol();
            }
        }
        else
        {
            if (hasBeenAttacked)
            {
                Attack();
                hasBeenAttacked = false;
            }
            else
            {
                Flee();
            }
        }

        if (performingTask)
        {
            PerformTask();
        }

        UpdateModelRotation();
        ApplyGravity();
    }

    void FindPlayerTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, chaseRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                currentPlayerTarget = hitCollider.transform;
                return; // Exit the loop once the player is found
            }
        }
    }

    void Chase()
    {
        if (currentPlayerTarget == null) return;

        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(currentPlayerTarget.position);
        animator.SetBool("isMoving", true);
        animator.SetTrigger("Chase");
    }

    void Attack()
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        animator.SetTrigger("attack");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Health health = hitCollider.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(attackDamage, gameObject);
                    UnityEngine.Debug.Log($"Dealt {attackDamage} damage to player");
                }
            }
        }

        lastAttackTime = Time.time;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (isDead) return;

        currentHealth -= damage;
        UnityEngine.Debug.Log($"Enemy took {damage} damage from {attacker.name}, remaining health: {currentHealth}");
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            hasBeenAttacked = true;
            FindPlayerTarget();
            float distanceToPlayer = Vector3.Distance(transform.position, currentPlayerTarget.position);
            if (distanceToPlayer < chaseRange)
            {
                Chase();
            }
        }
    }

    void CheckGroundStatus()
    {
        if (groundCheck == null) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0 || !isGrounded || isIdle) return;

        animator.SetBool("isMoving", true);

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

    void PerformTask()
    {
        if (taskWaypoints == null || taskWaypoints.Length == 0) return;

        animator.SetTrigger("performTask");
        navMeshAgent.SetDestination(taskWaypoints[currentTaskWaypointIndex].position);

        if (Vector3.Distance(transform.position, taskWaypoints[currentTaskWaypointIndex].position) < 0.2f)
        {
            currentTaskWaypointIndex = (currentTaskWaypointIndex + 1) % taskWaypoints.Length;
            performingTask = false; // Task completed, go back to patrolling
        }
    }

    IEnumerator IdlePause()
    {
        isIdle = true;
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(idleTime);
        isIdle = false;
        performingTask = true; // Switch to task mode after idle
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

    void Flee()
    {
        if (currentPlayerTarget == null) return;

        Vector3 directionAwayFromPlayer = transform.position - currentPlayerTarget.position;
        Vector3 newTargetPosition = transform.position + directionAwayFromPlayer.normalized * fleeDistance;
        navMeshAgent.SetDestination(newTargetPosition);
        animator.SetTrigger("Flee");
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("die");

        // Cache le PNJ au lieu de le désactiver
        SetRenderersVisible(false);

        StartCoroutine(DieAndRespawn());
    }

    void SetRenderersVisible(bool isVisible)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isVisible;
        }
    }

    IEnumerator DieAndRespawn()
    {
        // Attendre un délai avant de faire réapparaître le PNJ
        yield return new WaitForSeconds(2f); // Remplacez 2f par la durée souhaitée en secondes

        // Commence la routine de respawn après avoir caché le PNJ
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        transform.position = respawnPoint;
        currentHealth = maxHealth;
        isDead = false;

        // Rendre le PNJ visible à nouveau
        SetRenderersVisible(true);

        animator.SetTrigger("respawn");
    }

    void UpdateModelRotation()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f && isGrounded)
        {
            Vector3 direction = navMeshAgent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarEnemy != null)
        {
            healthBarEnemy.value = currentHealth;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}

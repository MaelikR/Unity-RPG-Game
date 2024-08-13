using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Collections;

public class HealthEnemy : NetworkBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthBarEnemy; // Assurez-vous que c'est bien r�f�renc�
    private Transform playerCamera; // R�f�rence � la cam�ra du joueur
    private string enemyType = "Skeleton";

    public AudioClip deathSound;
    public AudioSource audioSource;
    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;

        // Trouver la cam�ra du joueur
        FindPlayerCamera();

        // Initialiser les composants audio et animateur
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        if (healthBarEnemy != null)
        {
            healthBarEnemy.value = currentHealth / (float)maxHealth; // Initialisation correcte
        }
    }

    void Update()
    {
        // Faire en sorte que la barre de vie fasse face � la cam�ra du joueur
        if (playerCamera != null && healthBarEnemy != null)
        {
            healthBarEnemy.transform.parent.LookAt(playerCamera);
        }
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (currentHealth <= 0) return; // Ne rien faire si d�j� mort

        currentHealth -= damage;
        UnityEngine.Debug.Log($"Enemy took {damage} damage from {attacker.name}, remaining health: {currentHealth}");
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarEnemy != null)
        {
            healthBarEnemy.value = currentHealth / (float)maxHealth;
        }
    }

    private void Die()
    {
        ObjectiveManager objectiveManager = FindObjectiveManager();
        if (objectiveManager != null)
        {
            objectiveManager.RegisterEnemyKill(enemyType);
        }

        // Jouer le son de mort
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Jouer l'animation de mort
        if (animator != null)
        {
            animator.SetTrigger("die");
        }

        // D�truire l'objet apr�s un d�lai pour permettre l'animation de se terminer
        StartCoroutine(DestroyAfterDelay(3f)); // 3 secondes de d�lai, ajustez selon la dur�e de l'animation
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NetworkServer.Destroy(gameObject);
    }

    void FindPlayerCamera()
    {
        // Cherche la cam�ra du joueur (assure-toi que la cam�ra a un tag "MainCamera")
        Camera camera = Camera.main;
        if (camera != null)
        {
            playerCamera = camera.transform;
        }
    }

    ObjectiveManager FindObjectiveManager()
    {
        return FindObjectOfType<ObjectiveManager>();
    }
}

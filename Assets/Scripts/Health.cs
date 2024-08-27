using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Vector3 respawnPoint;
    private bool isDead = false;
    [SerializeField] public Slider healthBarFill;

    void Start()
    {
        currentHealth = maxHealth;
        respawnPoint = transform.position;
        UpdateHealthBar(maxHealth, currentHealth);
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (!isLocalPlayer || isDead) return;

        currentHealth -= damage;
        UnityEngine.Debug.Log($"Current Health: {currentHealth} after taking {damage} damage");
        UpdateHealthBar(maxHealth, currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayDamageFeedback();
        }
    }

    public void Heal(int amount)
    {
        if (!isLocalPlayer || isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar(maxHealth, currentHealth);
    }

    private void Die()
    {
        if (!isServer || isDead) return;

        isDead = true;
        if (gameObject.CompareTag("Player"))
        {
            StartCoroutine(RespawnPlayer());
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(RespawnEnemy());
        }

        StartCoroutine(DelayDeactivate());
    }

    [Server]
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(5f);
        currentHealth = maxHealth;
        RpcRespawn();
        isDead = false;
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            transform.position = respawnPoint;
            UpdateHealthBar(maxHealth, currentHealth);
        }
    }

    IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(5f);
        currentHealth = maxHealth;
        transform.position = respawnPoint;
        UpdateHealthBar(maxHealth, currentHealth);
        isDead = false;
    }

    IEnumerator DelayDeactivate()
    {
        yield return new WaitForSeconds(1f);
        // Here you can add logic to deactivate or destroy the object
    }

    private void PlayDamageFeedback()
    {
        // Implement visual or audio feedback here
    }

    private void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        if (healthBarFill != null)
        {
            healthBarFill.value = currentHealth / maxHealth;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}

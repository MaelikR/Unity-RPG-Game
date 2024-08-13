using UnityEngine;
using Mirror;

public class SwordController : NetworkBehaviour
{
    public Animator animator;
    public int swordDamage = 50;
    private bool isSwordDrawn = false;
    public float attackRadius = 1.2f;

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.E)) // Touche pour dégainer/rengainer
        {
            if (isSwordDrawn)
            {
                SheathSword();
            }
            else
            {
                DrawSword();
            }
        }

        if (isSwordDrawn && Input.GetMouseButtonDown(0)) // Touche pour attaquer
        {
            Attack();
            ApplyDamage();
        }
    }

    private void DrawSword()
    {
        isSwordDrawn = true;
        animator.SetBool("isSwordDrawn", true);
        animator.SetTrigger("DrawSword");
    }

    private void SheathSword()
    {
        isSwordDrawn = false;
        animator.SetBool("isSwordDrawn", false);
        animator.SetTrigger("SheathSword");
    }

    private void Attack()
    {
        animator.SetTrigger("isAttacking");
    }

    private void ApplyDamage()
    {
        // Code pour appliquer les dégâts à tous les ennemis dans la zone d'attaque
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && hitCollider.gameObject != gameObject)
            {
                Health playerHealth = hitCollider.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(swordDamage, gameObject);
                }
            }
            else if (hitCollider.CompareTag("Enemy"))
            {
                // Vérifiez si l'ennemi a un composant FFAAI et appliquez les dégâts
                FFAAI enemyAI = hitCollider.GetComponent<FFAAI>();
                if (enemyAI != null)
                {
                    UnityEngine.Debug.Log($"Dealt {swordDamage} damage to enemy using FFAAI");
                    enemyAI.TakeDamage(swordDamage, gameObject);
                }

                // Vérifiez si l'ennemi a un composant HealthEnemy et appliquez les dégâts
                HealthEnemy enemyHealth = hitCollider.GetComponent<HealthEnemy>();
                if (enemyHealth != null)
                {
                    UnityEngine.Debug.Log($"Dealt {swordDamage} damage to enemy using HealthEnemy");
                    enemyHealth.TakeDamage(swordDamage, gameObject);
                }
            }
        }
    }
}

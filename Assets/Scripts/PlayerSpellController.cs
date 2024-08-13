using UnityEngine;
using UnityEngine.UI; // Pour manipuler l'UI
using Mirror;
using System.Collections;


public class PlayerSpellController : NetworkBehaviour
{
    public GameObject spellPrefab; // Pr�fabriqu� du sort
    public Transform spellSpawnPoint; // Point de spawn du sort
    public float spellCooldown = 5f; // D�lai de r�cup�ration du sort
    private bool spellUnlocked = false;
    private bool isCooldown = false;
    private GameObject currentTarget;

    // R�f�rence � l'�l�ment UI pour indiquer la cible verrouill�e
    public Image targetLockUI;
    public Canvas canvas; // R�f�rence au Canvas de l'UI

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        // Activer le Canvas de l'UI seulement pour le joueur local
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogError("Canvas de l'UI n'est pas assign� dans l'inspecteur.");
        }
    }

    private void Update()
    {
        if (!isLocalPlayer || !spellUnlocked)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            LockTarget();
        }

        if (Input.GetMouseButtonDown(0) && currentTarget != null && !isCooldown)
        {
            CmdCastSpell(currentTarget.transform.position);
            StartCoroutine(SpellCooldown());
        }

        // Mettre � jour la position de l'UI de verrouillage de cible
        if (currentTarget != null && targetLockUI != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTarget.transform.position);
            targetLockUI.transform.position = screenPos;
            targetLockUI.enabled = true;
        }
        else if (targetLockUI != null)
        {
            targetLockUI.enabled = false;
        }
    }

    private void LockTarget()
    {
        // Code pour verrouiller une cible (� personnaliser selon votre jeu)
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                currentTarget = hit.transform.gameObject;
                if (targetLockUI != null)
                {
                    targetLockUI.enabled = true; // Activer l'UI de verrouillage de cible
                }
            }
        }
    }

    [Command]
    private void CmdCastSpell(Vector3 targetPosition)
    {
        RpcCastSpell(targetPosition);
    }

    [ClientRpc]
    private void RpcCastSpell(Vector3 targetPosition)
    {
        if (spellPrefab != null && spellSpawnPoint != null)
        {
            GameObject spell = Instantiate(spellPrefab, spellSpawnPoint.position, Quaternion.identity);
            spell.transform.LookAt(targetPosition);
            Rigidbody rb = spell.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = (targetPosition - spellSpawnPoint.position).normalized * 10f; // Vitesse du sort
            }

            Destroy(spell, 5f); // D�truire le sort apr�s 5 secondes
        }
    }

    public void UnlockSpell()
    {
        if (isLocalPlayer)
        {
            spellUnlocked = true;
        }
    }

    private IEnumerator SpellCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(spellCooldown);
        isCooldown = false;
    }
}

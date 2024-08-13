using UnityEngine;
using UnityEngine.Video;
using Mirror;
using System.Collections.Generic;
using System.Collections;
public class ArtifactVideoTrigger : NetworkBehaviour
{
    public VideoPlayer videoPlayer; // Référence au VideoPlayer à contrôler
    public GameObject player; // Référence au joueur
    public AudioClip unlockSound; // Son pour l'activation du sort
    public AudioSource audioSource; // Source audio pour jouer le son
    private bool hasTriggered = false; // Pour s'assurer que le trigger se déclenche une seule fois

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; // Si le trigger a déjà été activé, ne pas exécuter le reste

        if (other.CompareTag("Player"))
        {
            NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
            if (networkIdentity != null && networkIdentity.isLocalPlayer)
            {
                if (videoPlayer != null)
                {
                    videoPlayer.Play();
                }

                UnlockSpell();

                if (audioSource != null && unlockSound != null)
                {
                    audioSource.PlayOneShot(unlockSound);
                }

                hasTriggered = true; // Marquer le trigger comme activé
                DisableTrigger(); // Désactiver le trigger pour éviter des activations futures
            }
        }
    }

    private void UnlockSpell()
    {
        PlayerSpellController spellController = player.GetComponent<PlayerSpellController>();
        if (spellController != null)
        {
            spellController.UnlockSpell();
        }
    }

    private void DisableTrigger()
    {
        gameObject.SetActive(false); // Désactive l'artefact après utilisation
    }
}

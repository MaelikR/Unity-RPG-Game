using UnityEngine;
using UnityEngine.Video;
using Mirror;
using System.Collections.Generic;
using System.Collections;
public class ArtifactVideoTrigger : NetworkBehaviour
{
    public VideoPlayer videoPlayer; // R�f�rence au VideoPlayer � contr�ler
    public GameObject player; // R�f�rence au joueur
    public AudioClip unlockSound; // Son pour l'activation du sort
    public AudioSource audioSource; // Source audio pour jouer le son
    private bool hasTriggered = false; // Pour s'assurer que le trigger se d�clenche une seule fois

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; // Si le trigger a d�j� �t� activ�, ne pas ex�cuter le reste

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

                hasTriggered = true; // Marquer le trigger comme activ�
                DisableTrigger(); // D�sactiver le trigger pour �viter des activations futures
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
        gameObject.SetActive(false); // D�sactive l'artefact apr�s utilisation
    }
}

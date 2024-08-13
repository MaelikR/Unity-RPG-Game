using UnityEngine;
using UnityEngine.Video;
using Mirror;
using System.Collections;
public class LocalPlayerVideoTrigger : NetworkBehaviour
{
	public VideoPlayer videoPlayer; // Référence au VideoPlayer à contrôler
	public bool playOnEnter = true; // Lire la vidéo à l'entrée
	public bool stopOnExit = true; // Arrêter la vidéo à la sortie

	private bool hasTriggered = false; // Pour s'assurer que le trigger se déclenche une seule fois

	private void OnTriggerEnter(Collider other)
	{
		if (hasTriggered) return; // Si le trigger a déjà été activé, ne pas exécuter le reste

		if (other.CompareTag("Player"))
		{
			NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
			if (networkIdentity != null && networkIdentity.isLocalPlayer)
			{
				if (playOnEnter && videoPlayer != null)
				{
					videoPlayer.Play();
				}

				hasTriggered = true; // Marquer le trigger comme activé
				DisableTrigger(); // Désactiver le trigger pour éviter des activations futures
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!hasTriggered) return; // Si le trigger n'a pas été activé, ne pas exécuter le reste

		if (other.CompareTag("Player"))
		{
			NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
			if (networkIdentity != null && networkIdentity.isLocalPlayer)
			{
				if (stopOnExit && videoPlayer != null)
				{
					videoPlayer.Stop();
				}
			}
		}
	}

	private void DisableTrigger()
	{
		// Désactiver le BoxCollider pour empêcher d'autres activations
		GetComponent<Collider>().enabled = false;
	}
}

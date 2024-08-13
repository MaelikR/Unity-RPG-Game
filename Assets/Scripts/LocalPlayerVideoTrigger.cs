using UnityEngine;
using UnityEngine.Video;
using Mirror;
using System.Collections;
public class LocalPlayerVideoTrigger : NetworkBehaviour
{
	public VideoPlayer videoPlayer; // R�f�rence au VideoPlayer � contr�ler
	public bool playOnEnter = true; // Lire la vid�o � l'entr�e
	public bool stopOnExit = true; // Arr�ter la vid�o � la sortie

	private bool hasTriggered = false; // Pour s'assurer que le trigger se d�clenche une seule fois

	private void OnTriggerEnter(Collider other)
	{
		if (hasTriggered) return; // Si le trigger a d�j� �t� activ�, ne pas ex�cuter le reste

		if (other.CompareTag("Player"))
		{
			NetworkIdentity networkIdentity = other.GetComponent<NetworkIdentity>();
			if (networkIdentity != null && networkIdentity.isLocalPlayer)
			{
				if (playOnEnter && videoPlayer != null)
				{
					videoPlayer.Play();
				}

				hasTriggered = true; // Marquer le trigger comme activ�
				DisableTrigger(); // D�sactiver le trigger pour �viter des activations futures
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!hasTriggered) return; // Si le trigger n'a pas �t� activ�, ne pas ex�cuter le reste

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
		// D�sactiver le BoxCollider pour emp�cher d'autres activations
		GetComponent<Collider>().enabled = false;
	}
}

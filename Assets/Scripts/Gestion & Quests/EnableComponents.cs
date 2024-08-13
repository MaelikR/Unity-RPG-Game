using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;
using Debug = UnityEngine.Debug; // Résoudre le conflit de nom

namespace StarterAssets
{
    public class EnableComponents : NetworkBehaviour
    {
        public Transform target;
        private CinemachineBrain cinemachineBrain; // Déclaration au niveau de la classe

        private void Start()
        {
            if (isLocalPlayer)
            {
                CharacterController cController = GetComponent<CharacterController>();
                cController.enabled = true;

                ThirdPersonController TPC = GetComponent<ThirdPersonController>();
                TPC.enabled = true;

                // Obtenir la référence au Cinemachine Brain attaché à la caméra principale
                cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

                // Assurez-vous que le Cinemachine Brain existe
                if (cinemachineBrain == null)
                {
                    Debug.LogError("Aucun Cinemachine Brain trouvé sur la caméra principale.");
                }
                else
                {
                    cinemachineBrain.enabled = true; // Activer le Cinemachine Brain
                }

                PlayerInput input = GetComponent<PlayerInput>();
                input.enabled = true;

                CinemachineFreeLook cinemachineFreeLook = GetComponentInChildren<CinemachineFreeLook>();
                if (cinemachineFreeLook != null)
                {
                    cinemachineFreeLook.enabled = true;
                }
            }
        }
    }
}

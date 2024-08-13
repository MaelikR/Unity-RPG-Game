using UnityEngine;
using Cinemachine;
using Mirror;
using System.Diagnostics;

public class CameraCollisionHandler : NetworkBehaviour
{
    public Transform target; // Le transform du joueur ou de l'objet suivi par la caméra
    public LayerMask collisionLayers; // Les couches avec lesquelles la caméra peut entrer en collision
    public float cameraRadius = 0.5f; // Rayon de la sphère de collision
    public float smoothTime = 0.2f; // Temps de lissage de la position de la caméra
    public Vector3 offset; // Décalage de la caméra par rapport à la cible

    private Vector3 currentVelocity;

    // Déclarez la variable pour la caméra virtuelle
    private CinemachineVirtualCamera virtualCamera;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Assurez-vous que ce script ne s'exécute que pour le joueur local
        if (isLocalPlayer)
        {
            // Trouver la caméra virtuelle attachée au joueur instancié
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

            if (virtualCamera == null)
            {
                UnityEngine.Debug.LogError("CinemachineVirtualCamera is not found!");
                return;
            }

            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

     
    }

    void LateUpdate()
    {
        if (!isLocalPlayer || virtualCamera == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        RaycastHit hit;

        if (Physics.SphereCast(target.position, cameraRadius, (desiredPosition - target.position).normalized, out hit, Vector3.Distance(target.position, desiredPosition), collisionLayers))
        {
            Vector3 collisionPoint = hit.point;
            Vector3 collisionNormal = hit.normal;

            desiredPosition = collisionPoint + collisionNormal * cameraRadius;
        }

        virtualCamera.transform.position = Vector3.SmoothDamp(virtualCamera.transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }
}

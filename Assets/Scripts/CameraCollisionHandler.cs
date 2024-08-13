using UnityEngine;
using Cinemachine;
using Mirror;
using System.Diagnostics;

public class CameraCollisionHandler : NetworkBehaviour
{
    public Transform target; // Le transform du joueur ou de l'objet suivi par la cam�ra
    public LayerMask collisionLayers; // Les couches avec lesquelles la cam�ra peut entrer en collision
    public float cameraRadius = 0.5f; // Rayon de la sph�re de collision
    public float smoothTime = 0.2f; // Temps de lissage de la position de la cam�ra
    public Vector3 offset; // D�calage de la cam�ra par rapport � la cible

    private Vector3 currentVelocity;

    // D�clarez la variable pour la cam�ra virtuelle
    private CinemachineVirtualCamera virtualCamera;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Assurez-vous que ce script ne s'ex�cute que pour le joueur local
        if (isLocalPlayer)
        {
            // Trouver la cam�ra virtuelle attach�e au joueur instanci�
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

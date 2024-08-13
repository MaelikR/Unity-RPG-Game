using UnityEngine;
using Mirror;

public class SunRotation : NetworkBehaviour
{
    [SyncVar]
    public float rotationSpeed = 10.0f;

    void Update()
    {
        if (isServer)
        {
            RotateSun();
        }
    }

    [Server]
    private void RotateSun()
    {
        // Rotation autour de l'axe Y (vers le haut)
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);

        // Synchroniser la rotation sur tous les clients
        RpcSyncRotation(transform.rotation);
    }

    [ClientRpc]
    private void RpcSyncRotation(Quaternion rotation)
    {
        // Appliquer la rotation synchronisée sur le client
        transform.rotation = rotation;
    }

    [Command]
    public void CmdSetRotationSpeed(float newRotationSpeed)
    {
        rotationSpeed = newRotationSpeed;
    }
}

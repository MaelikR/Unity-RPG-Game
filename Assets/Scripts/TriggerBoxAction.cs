using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class TriggerBoxAction : NetworkBehaviour
{
    public Camera travellingCamera1;
    public Transform travelStartPoint1;
    public Transform travelEndPoint1;
    public string messageToShow1 = "";

    public Camera travellingCamera2;
    public Transform travelStartPoint2;
    public Transform travelEndPoint2;
    public string messageToShow2 = "";

    public float cameraTravelDuration = 3.0f;
    public Text uiText;  // Reference to the UI Text component
    public GameObject Player;
    private HashSet<NetworkIdentity> triggeredPlayers = new HashSet<NetworkIdentity>();

    private void Start()
    {
        // Ensure the travelling cameras are initially disabled
        travellingCamera1.enabled = false;
        travellingCamera2.enabled = false;

        // Ensure the UI Text is initially disabled
        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return; // Only the server should handle the trigger

        if (other.CompareTag("Player"))
        {
            NetworkIdentity playerIdentity = other.GetComponent<NetworkIdentity>();
            if (playerIdentity != null && !triggeredPlayers.Contains(playerIdentity))
            {
                // Mark this player as having triggered the event
                triggeredPlayers.Add(playerIdentity);

                // Start the camera travelling and text display coroutine
                StartCoroutine(HandleTriggerEvent(playerIdentity));
            }
        }
    }

    private IEnumerator HandleTriggerEvent(NetworkIdentity playerIdentity)
    {
        UnityEngine.Debug.Log("Trigger event started for player: " + playerIdentity.netId);

        RpcShowMessage(playerIdentity.connectionToClient, messageToShow1);
        RpcStartCameraTravel(playerIdentity.connectionToClient, 1);

        yield return new WaitForSeconds(cameraTravelDuration);

        RpcEndCameraTravel(playerIdentity.connectionToClient, 1);

        yield return new WaitForSeconds(1.0f); // Small delay before starting the second camera travel

        RpcShowMessage(playerIdentity.connectionToClient, messageToShow2);
        RpcStartCameraTravel(playerIdentity.connectionToClient, 2);

        yield return new WaitForSeconds(cameraTravelDuration);

        RpcEndCameraTravel(playerIdentity.connectionToClient, 2);

        UnityEngine.Debug.Log("Trigger event ended for player: " + playerIdentity.netId);
    }


    [TargetRpc]
    void RpcShowMessage(NetworkConnection target, string message)
    {
        UnityEngine.Debug.Log("Showing message: " + message);

        // Display the message
        if (uiText != null)
        {
            uiText.text = message;
            uiText.gameObject.SetActive(true);
        }
    }

    [TargetRpc]
    void RpcStartCameraTravel(NetworkConnection target, int cameraIndex)
    {
        UnityEngine.Debug.Log("Starting camera travel for camera index: " + cameraIndex);

        Camera travellingCamera = cameraIndex == 1 ? travellingCamera1 : travellingCamera2;
        Transform startPoint = cameraIndex == 1 ? travelStartPoint1 : travelStartPoint2;
        Transform endPoint = cameraIndex == 1 ? travelEndPoint1 : travelEndPoint2;

        if (travellingCamera != null)
        {
            travellingCamera.enabled = true;
            StartCoroutine(TravelCamera(travellingCamera, startPoint, endPoint));
        }
    }

    private IEnumerator TravelCamera(Camera camera, Transform startPoint, Transform endPoint)
    {
        float elapsedTime = 0f;

        while (elapsedTime < cameraTravelDuration)
        {
            camera.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, elapsedTime / cameraTravelDuration);
            camera.transform.rotation = Quaternion.Lerp(startPoint.rotation, endPoint.rotation, elapsedTime / cameraTravelDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        camera.transform.position = endPoint.position;
        camera.transform.rotation = endPoint.rotation;

        UnityEngine.Debug.Log("Camera travel completed for: " + camera.name);
    }

    [TargetRpc]
    void RpcEndCameraTravel(NetworkConnection target, int cameraIndex)
    {
        UnityEngine.Debug.Log("Ending camera travel for camera index: " + cameraIndex);

        Camera travellingCamera = cameraIndex == 1 ? travellingCamera1 : travellingCamera2;

        if (travellingCamera != null)
        {
            travellingCamera.enabled = false;
        }

        // Hide the message
        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
/*
 * =============================================================
 * Script: TriggerBoxActionTravellingLinearZoom
 * Author: M.Ren
 * Date: [26/08/2024]
 * 
 * Description:
 * --------------
 * This script controls the activation of three cameras in a Unity 
 * scene to perform linear travel movements with zoom effects.
 * Each camera moves between two specified points while displaying 
 * a message on the screen, creating an immersive narrative 
 * experience for the player. The script uses Mirror for network 
 * management, ensuring synchronization of events between clients 
 * and the server.
 * 
 * Features:
 * ----------------
 * - Controls three cameras with linear movement and zoom effects.
 * - Displays synchronized messages alongside camera movements.
 * - Synchronizes events using targeted RPCs.
 * 
 * Usage:
 * ------------
 * 1. Assign the cameras, start points, end points, and messages 
 *    to display through the Unity inspector.
 * 2. The script will trigger camera movements and message displays 
 *    when a player enters the trigger zone.
 * 
 * Note:
 * ---------
 * This script is designed to be used in a networked environment 
 * where only the server instance handles triggers.
 * =============================================================
 */
public class TriggerBoxActionTravellingLinearZoom : NetworkBehaviour
{
    public Camera travellingCamera1;
    public Transform travelStartPoint1;
    public Transform travelEndPoint1;
    public string messageToShow1 = "text cam 01";

    public Camera travellingCamera2;
    public Transform travelStartPoint2;
    public Transform travelEndPoint2;
    public string messageToShow2 = "text cam 02";

    public Camera travellingCamera3;
    public Transform travelStartPoint3;
    public Transform travelEndPoint3;
    public string messageToShow3 = "text cam 03";

    public float cameraTravelDuration = 3.0f;
    public Text uiText;  // Reference to the UI Text component
    public GameObject Player;
    private HashSet<NetworkIdentity> triggeredPlayers = new HashSet<NetworkIdentity>();

    private void Start()
    {
        // Ensure the travelling cameras are initially disabled
        travellingCamera1.enabled = true;
        travellingCamera2.enabled = false;
        travellingCamera3.enabled = false;
        // Ensure the UI Text is initially disabled
        if (uiText != null)
        {
            uiText.gameObject.SetActive(true);
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

        yield return new WaitForSeconds(1.0f); // Small delay before starting the third camera travel

        RpcShowMessage(playerIdentity.connectionToClient, messageToShow3);
        RpcStartCameraTravel(playerIdentity.connectionToClient, 3);

        yield return new WaitForSeconds(cameraTravelDuration);

        RpcEndCameraTravel(playerIdentity.connectionToClient, 3);

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

    Camera travellingCamera;
    Transform startPoint;
    Transform endPoint;

    switch (cameraIndex)
    {
        case 1:
            travellingCamera = travellingCamera1;
            startPoint = travelStartPoint1;
            endPoint = travelEndPoint1;
            break;
        case 2:
            travellingCamera = travellingCamera2;
            startPoint = travelStartPoint2;
            endPoint = travelEndPoint2;
            break;
        case 3:
            travellingCamera = travellingCamera3;
            startPoint = travelStartPoint3;
            endPoint = travelEndPoint3;
            break;
        default:
            return; // Exit if the camera index is not valid
    }

    if (travellingCamera != null)
    {
        travellingCamera.enabled = true;
        StartCoroutine(LinearTravelWithZoom(travellingCamera, startPoint, endPoint, travellingCamera.fieldOfView, 30f, cameraTravelDuration));
    }
}

    private IEnumerator LinearTravelWithZoom(Camera camera, Transform startPoint, Transform endPoint, float startFOV, float endFOV, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            camera.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, elapsedTime / duration);
            camera.transform.rotation = Quaternion.Lerp(startPoint.rotation, endPoint.rotation, elapsedTime / duration);
            camera.fieldOfView = Mathf.Lerp(startFOV, endFOV, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        camera.transform.position = endPoint.position;
        camera.transform.rotation = endPoint.rotation;
        camera.fieldOfView = endFOV;
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

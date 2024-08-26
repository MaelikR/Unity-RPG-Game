using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
/*
 * =============================================================
 * Script: TriggerBoxActionTravellingLinearZoom
 * Author: M.Ren]
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
    public string messageToShow1 = "the dark forces invade the world at a crazy speed we must act quickly the first and last moments for men have rung we must also gather the black stones in order to pass the dark door of the primordial demon as in each cycle of destruction";

    public Camera travellingCamera2;
    public Transform travelStartPoint2;
    public Transform travelEndPoint2;
    public string messageToShow2 = "As you also fear them, the first great forces of the demonic spawns are making their appearances everywhere in the world in order to protect the stones during the cycle and appear at the end of the invocation of their master, you have little time left, you must act before it is too late...";

    public Camera travellingCamera3;
    public Transform travelStartPoint3;
    public Transform travelEndPoint3;
    public string messageToShow3 = "Here is a superior demon invisible to the eyes of men but extremely dangerous for those who confront them in their dimensions, they serve to protect the different sites of the magic stones across their dimensions in order to prevent any source of magic that could interact with the ritual of destruction of the primordial demon.";

    public float cameraTravelDuration = 3.0f;
    public Text uiText;  // Reference to the UI Text component
    private HashSet<NetworkIdentity> triggeredPlayers = new HashSet<NetworkIdentity>();

    private void Start()
    {
        // Ensure the travelling cameras are initially disabled
        travellingCamera1.enabled = false;
        travellingCamera2.enabled = false;
        travellingCamera3.enabled = false;
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

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return; // Only the server should handle the trigger

        if (other.CompareTag("Player"))
        {
            NetworkIdentity playerIdentity = other.GetComponent<NetworkIdentity>();
            if (playerIdentity != null && triggeredPlayers.Contains(playerIdentity))
            {
                // When the player exits the trigger, end the camera travel and remove from the triggered list
                triggeredPlayers.Remove(playerIdentity);
                StopCoroutine(HandleTriggerEvent(playerIdentity));
                RpcEndAllCameraTravels(playerIdentity.connectionToClient);
            }
        }
    }

    private IEnumerator HandleTriggerEvent(NetworkIdentity playerIdentity)
    {
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
    }

    [TargetRpc]
    void RpcShowMessage(NetworkConnection target, string message)
    {
        if (uiText != null)
        {
            uiText.text = message;
            uiText.gameObject.SetActive(true);
        }
    }

    [TargetRpc]
    void RpcStartCameraTravel(NetworkConnection target, int cameraIndex)
    {
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
        Camera travellingCamera = null;
        switch (cameraIndex)
        {
            case 1:
                travellingCamera = travellingCamera1;
                break;
            case 2:
                travellingCamera = travellingCamera2;
                break;
            case 3:
                travellingCamera = travellingCamera3;
                break;
        }

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

    [TargetRpc]
    void RpcEndAllCameraTravels(NetworkConnection target)
    {
        travellingCamera1.enabled = false;
        travellingCamera2.enabled = false;
        travellingCamera3.enabled = false;

        // Hide the message
        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
    }
}

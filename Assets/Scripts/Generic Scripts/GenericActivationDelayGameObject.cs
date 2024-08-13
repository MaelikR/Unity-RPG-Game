using UnityEngine;
using Mirror;

public class GenericActivationDelayGameObject : NetworkBehaviour
{
    public GameObject[] objectsToActivate; // Tableau de GameObjects à activer
    [Range(0f, 60f)]
    public float activedDlyTime = 20f;

    private void Start()
    {
        if (isLocalPlayer)
        {
            Invoke("ActivationGameObjects", activedDlyTime);
        }
    }

    private void ActivationGameObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }
    }
}

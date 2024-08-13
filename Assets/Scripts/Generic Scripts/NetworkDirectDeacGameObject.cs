using UnityEngine;
using Mirror;

public class NetworkDirectDeacGameObject : NetworkBehaviour
{
	public GameObject NetdeacObject;

	private void Start()
	{
        if (!isLocalPlayer)
        {
            NetdeacObject.SetActive(false);
        }
	}
}
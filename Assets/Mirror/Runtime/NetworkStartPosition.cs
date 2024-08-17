using UnityEngine;

namespace Mirror
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Network Start Position")]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-start-position")]
    public class NetworkStartPosition : MonoBehaviour
    {
        public NetworkStartPositionManager.SpawnType spawnType;

        void Awake()
        {
            NetworkStartPositionManager manager = FindObjectOfType<NetworkStartPositionManager>();
            if (manager != null)
            {
                manager.RegisterStartPosition(transform, spawnType);
            }
        }

        void OnDestroy()
        {
            NetworkStartPositionManager manager = FindObjectOfType<NetworkStartPositionManager>();
            if (manager != null)
            {
                manager.UnRegisterStartPosition(transform, spawnType);
            }
        }
    }
}

using UnityEngine;
using Mirror;

public class PlayerNotifier : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        NotifyEnemies();
    }

    void NotifyEnemies()
    {
        if (!isLocalPlayer) return;

        SkeletonAI[] enemies = FindObjectsOfType<SkeletonAI>();
        foreach (SkeletonAI enemy in enemies)
        {
            enemy.RegisterPlayer(gameObject);
        }
    }
}

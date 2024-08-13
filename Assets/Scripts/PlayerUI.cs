using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    public Health playerHealth;
    public Slider healthBarFill;

    void Update()
    {
        if (!isLocalPlayer) return;

        if (healthBarFill != null && playerHealth != null)
        {
            healthBarFill.value = playerHealth.GetCurrentHealth() / (float)playerHealth.maxHealth;
        }
    }
}

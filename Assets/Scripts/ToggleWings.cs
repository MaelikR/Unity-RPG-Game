using UnityEngine;

public class ToggleWings : MonoBehaviour
{
    // R�f�rence au GameObject repr�sentant les ailes
    public GameObject wings;

    // Variable pour garder la trace de l'�tat des ailes
    private bool wingsActive = true;

    // M�thode appel�e une fois par frame
    void Update()
    {
        // V�rifie si la touche "C" est press�e
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Bascule l'�tat des ailes
            wingsActive = !wingsActive;

            // Active ou d�sactive le GameObject des ailes en fonction de l'�tat
            wings.SetActive(wingsActive);
        }
    }
}

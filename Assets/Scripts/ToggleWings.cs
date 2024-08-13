using UnityEngine;

public class ToggleWings : MonoBehaviour
{
    // Référence au GameObject représentant les ailes
    public GameObject wings;

    // Variable pour garder la trace de l'état des ailes
    private bool wingsActive = true;

    // Méthode appelée une fois par frame
    void Update()
    {
        // Vérifie si la touche "C" est pressée
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Bascule l'état des ailes
            wingsActive = !wingsActive;

            // Active ou désactive le GameObject des ailes en fonction de l'état
            wings.SetActive(wingsActive);
        }
    }
}

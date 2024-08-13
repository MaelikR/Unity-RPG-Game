using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{
    public Slider healthSlider; // Référence au Slider UI
    public GameObject healthUI; // Référence au GameObject contenant la barre de vie
    public FFAAI ai; // Référence au script du PNJ
    private Transform playerCamera; // Référence à la caméra du joueur

    void Start()
    {
        if (ai == null)
        {
            UnityEngine.Debug.LogError("Le script FFAAI n'est pas assigné.");
            return;
        }

        // Trouver la caméra du joueur
        FindPlayerCamera();

        if (healthSlider != null)
        {
            // Configurer le Slider avec les valeurs initiales
            healthSlider.maxValue = ai.maxHealth;
            healthSlider.value = ai.currentHealth;
        }

        if (healthUI != null)
        {
            // Assurez-vous que la barre de vie est visible au début
            healthUI.SetActive(true);
        }
    }

    void Update()
    {
        if (ai != null)
        {
            if (ai.isDead)
            {
                // Cacher la barre de vie si le PNJ est mort
                if (healthUI != null)
                {
                    healthUI.SetActive(false);
                }
            }
            else
            {
                // Mettre à jour le Slider en fonction de la santé actuelle du PNJ
                if (healthSlider != null)
                {
                    healthSlider.value = ai.currentHealth;
                }

                // Assurez-vous que la barre de vie est visible si le PNJ est vivant
                if (healthUI != null)
                {
                    healthUI.SetActive(true);
                }

                // Faire en sorte que la barre de vie fasse face à la caméra du joueur
                if (playerCamera != null)
                {
                    healthUI.transform.LookAt(playerCamera);
                }
            }
        }
    }

    void FindPlayerCamera()
    {
        // Cherche la caméra du joueur (assure-toi que la caméra a un tag "MainCamera")
        Camera camera = Camera.main;
        if (camera != null)
        {
            playerCamera = camera.transform;
        }
    }
}

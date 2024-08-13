using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{
    public Slider healthSlider; // R�f�rence au Slider UI
    public GameObject healthUI; // R�f�rence au GameObject contenant la barre de vie
    public FFAAI ai; // R�f�rence au script du PNJ
    private Transform playerCamera; // R�f�rence � la cam�ra du joueur

    void Start()
    {
        if (ai == null)
        {
            UnityEngine.Debug.LogError("Le script FFAAI n'est pas assign�.");
            return;
        }

        // Trouver la cam�ra du joueur
        FindPlayerCamera();

        if (healthSlider != null)
        {
            // Configurer le Slider avec les valeurs initiales
            healthSlider.maxValue = ai.maxHealth;
            healthSlider.value = ai.currentHealth;
        }

        if (healthUI != null)
        {
            // Assurez-vous que la barre de vie est visible au d�but
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
                // Mettre � jour le Slider en fonction de la sant� actuelle du PNJ
                if (healthSlider != null)
                {
                    healthSlider.value = ai.currentHealth;
                }

                // Assurez-vous que la barre de vie est visible si le PNJ est vivant
                if (healthUI != null)
                {
                    healthUI.SetActive(true);
                }

                // Faire en sorte que la barre de vie fasse face � la cam�ra du joueur
                if (playerCamera != null)
                {
                    healthUI.transform.LookAt(playerCamera);
                }
            }
        }
    }

    void FindPlayerCamera()
    {
        // Cherche la cam�ra du joueur (assure-toi que la cam�ra a un tag "MainCamera")
        Camera camera = Camera.main;
        if (camera != null)
        {
            playerCamera = camera.transform;
        }
    }
}

using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    public Vector3 generationArea = new Vector3(50f, 0f, 50f);
    public GameObject rockPrefab;
    public int numberOfRocks = 10;

    public void GenerateRocks()
    {
        Terrain terrain = Terrain.activeTerrain;

        if (terrain == null)
        {
            Debug.LogError("No active terrain found.");
            return;
        }

        float terrainHeight = terrain.transform.position.y; // Obtenez la hauteur du terrain

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-generationArea.x / 2, generationArea.x / 2),
                0f,
                Random.Range(-generationArea.z / 2, generationArea.z / 2)
            );

            // Obtenez la hauteur du terrain à la position
            float sampleHeight = terrain.SampleHeight(transform.position + randomPosition);

            // Ajoutez la hauteur du terrain à la position Y du rocher
            randomPosition.y = terrainHeight + sampleHeight;

            // Descendez l'axe Y
            float yOffset = Random.Range(-20f, -10f); // Ajustez ces valeurs en fonction de votre besoin
            randomPosition.y += yOffset;

            // Rotation aléatoire
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Échelle différente aléatoire
            float randomScale = Random.Range(2.5f, 25f);
            Vector3 randomScaleVector = new Vector3(randomScale, randomScale, randomScale);

            Instantiate(rockPrefab, transform.position + randomPosition, randomRotation).transform.localScale = randomScaleVector;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public Text timerText;
    public Transform keyPoint;
    private float timeRemaining;
    private bool timerIsRunning = true;

    void Start()
    {
        timeRemaining = 0f; // Initialiser le timer à 0
    }

    void Update()
    {
        if (timerIsRunning)
        {
            timeRemaining += Time.deltaTime;
            DisplayTime(timeRemaining);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == keyPoint)
        {
            timerIsRunning = false;
            Debug.Log("Arrivé au point clé !");
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

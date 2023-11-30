using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 30f; 
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    public GameObject targetNPC;
    public GameObject player; 

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timerIsRunning = false;
                timeRemaining = 0;
                CheckNPCStatus();
            }
        }
    }
    void CheckNPCStatus()
    {
        if (targetNPC != null && targetNPC.activeSelf)
        {
            
            OnPlayerDeath();
        }
    }
     void OnPlayerDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}", seconds);
    }

 
    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }
}
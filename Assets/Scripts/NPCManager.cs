using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public GameObject npcDeath; // The NPC that needs to be killed
    public GameObject newNPC; // The new NPC
    public Timer gameTimer; // Reference to the Timer script

    void Update()
    {
        if (npcDeath == null)
        {
            if (newNPC != null && !newNPC.activeSelf)
            {
                newNPC.SetActive(true);
                gameTimer.targetNPC = newNPC; // Assign the new NPC to the timer
                gameTimer.StartTimer(); // Start the timer
            }
        }
    }
}
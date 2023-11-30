using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public GameObject npcDeath; 
    public GameObject newNPC; 
    public Timer gameTimer; 

    void Update()
    {
        if (npcDeath == null)
        {
            if (newNPC != null && !newNPC.activeSelf)
            {
                newNPC.SetActive(true);
                gameTimer.targetNPC = newNPC; 
                gameTimer.StartTimer(); 
            }
        }
    }
}
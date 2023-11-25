using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class NPC : MonoBehaviour
{
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI healthText;
    public string npcID = "NPC";
    public enum NPCStates
    {
        Patrol,
        Chase,
        Attack,
        Retreat,
        HealRetreat
    }

    [SerializeField] Vector3[] PatrolPoints;
    [SerializeField]Transform Player;
    [SerializeField] Bullet Bullet;
    [SerializeField] Material PatrolMaterial;
    [SerializeField] Material ChaseMaterial;
    [SerializeField] Material AttackMaterial;
    [SerializeField] Material RetreatMaterial;
    [SerializeField] float ChaseRange = 7f;
    [SerializeField] float AttackRange = 4f;

 
    int nextPatrolPoint = 0;
    NPCStates currentState = NPCStates.Patrol;
    NavMeshAgent navMeshAgent;
    MeshRenderer meshRenderer;

    float nextFire;
   
    float FireRate = 1f;
    public Transform bulletPosition;
    public GameObject bulletPrefab;
    public int health = 100;

   

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
     
        navMeshAgent.SetDestination(PatrolPoints[nextPatrolPoint]);
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return; // Ensure no further code is executed after destruction
        }

        if (health <= 20 && currentState != NPCStates.HealRetreat)
        {
            currentState = NPCStates.HealRetreat;
        }
        else
        {
            SwitchState(); // Use the probability-based method
        }

        UpdateStateText();
        if (stateText != null)
        {
            stateText.text = $"{npcID} State: {currentState}";
            Debug.Log("Updated Text: " + stateText.text);
            if (healthText != null)
            {
                healthText.text = "HP " + health;
            }
        }
    
    }

    void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + FireRate;

            GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);
            bullet.GetComponent<Bullet>()?.InitializeBullet(transform.rotation * Vector3.forward);

        }
    }
    private void SwitchState()
    {
        switch (currentState)
        {
            case NPCStates.Patrol:
                Patrol();
                break;
            case NPCStates.Chase:
                Chase();
                break;
            case NPCStates.Attack:
                Attack();
                break;
            case NPCStates.HealRetreat:
                HealRetreat();
                break;
            case NPCStates.Retreat:
                Retreat();
                break;
            default:
                Patrol();
                break;
        }
    }

    private void Attack()
    {
        navMeshAgent.ResetPath(); // Reset the NPC's path
        navMeshAgent.isStopped = true; // Explicitly stop the NPC's movement
        meshRenderer.material = AttackMaterial; // Change material to attack material
        transform.LookAt(Player); // Ensure the NPC looks at the player

        Fire(); // Attempt to fire a bullet

        // Check if the player is out of attack range
        if (Vector3.Distance(transform.position, Player.position) > AttackRange)
        {
            navMeshAgent.isStopped = false; // Allow the NPC to move again
            currentState = NPCStates.Chase;
        }
    }

    private void Chase()
    {
        meshRenderer.material = ChaseMaterial; // Change material to chase material
        navMeshAgent.SetDestination(Player.position); // Set destination to player's position

        // Check if player is within attack range
        if (Vector3.Distance(transform.position, Player.position) <= AttackRange)
        {
            currentState = NPCStates.Attack;
        }
        // Check if player is out of chase range
        else if (Vector3.Distance(transform.position, Player.position) > ChaseRange)
        {
            navMeshAgent.ResetPath(); // Reset the path
            currentState = NPCStates.Patrol;
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, Player.position) < ChaseRange)
        {
            currentState = NPCStates.Chase;
            return; // Exit the method to avoid executing remaining code when state changes
        }

        // If the NPC is not moving
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            meshRenderer.material = PatrolMaterial; // Change material to patrol material

            // Increment patrol point and reset if needed
            nextPatrolPoint = (nextPatrolPoint + 1) % PatrolPoints.Length;
            navMeshAgent.SetDestination(PatrolPoints[nextPatrolPoint]);
        }
    }

    private void Retreat()
    {

        int farthestPointIndex = 0;
        float maxDistance = 0;

        // Find the farthest patrol point from the player
        for (int i = 0; i < PatrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(PatrolPoints[i], Player.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPointIndex = i;
            }
        }

        // Set the destination to the farthest patrol point
        navMeshAgent.isStopped = false; // Ensure the agent can move
        navMeshAgent.SetDestination(PatrolPoints[farthestPointIndex]);
        meshRenderer.material = RetreatMaterial; // Change to retreat material
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            currentState = NPCStates.Patrol; // Transition back to Patrol state
            meshRenderer.material = PatrolMaterial; // Change material to patrol material
            navMeshAgent.SetDestination(PatrolPoints[nextPatrolPoint]); // Reset to the next patrol point
        }
        // Debugging
        Debug.Log("Retreating to point: " + farthestPointIndex);
    }
    private void HealRetreat()
    {
        int farthestPointIndex = 0;
        float maxDistance = 0;
        for (int i = 0; i < PatrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(PatrolPoints[i], Player.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPointIndex = i;
            }
        }

        // Set the destination to the farthest patrol point
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(PatrolPoints[farthestPointIndex]);
        meshRenderer.material = RetreatMaterial; // You can choose to have a different material for HealRetreat if you like

        // Check if NPC has reached the retreat point and heal if necessary
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            if (health < 100)
            {
                health++; // Slowly regenerate health
            }
            else
            {
                currentState = NPCStates.Patrol; // Return to patrol once healed
                meshRenderer.material = PatrolMaterial; // Reset material to patrol material
            }
        }
    }
    private void UpdateStateText()
    {
        if (stateText != null)
        {
            stateText.text = $" State: {currentState}";
            Debug.Log("Updated Text: " + stateText.text); // Debugging line
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Seeker")) // Make sure the player has a tag "Player"
        {
            currentState = NPCStates.Retreat; // Change state to Retreat
        }
    
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            TakeDamage(bullet.damage);
        }
    }
    void TakeDamage(int damage)
    {
        Debug.Log("Damage Taken: " + damage);
        health -= damage;
        Debug.Log("New Health: " + health);
        if (healthText != null)
        {
            healthText.text = "HP " + health;
        }

       
    }
}


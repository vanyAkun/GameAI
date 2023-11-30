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

    public NPC leader;
    private bool isLeader = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
     
        navMeshAgent.SetDestination(PatrolPoints[nextPatrolPoint]);
        meshRenderer = GetComponent<MeshRenderer>();

        isLeader = leader == null;
    }
    
    
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return; 
        }

        if (health <= 20 && currentState != NPCStates.HealRetreat)
        {
            currentState = NPCStates.HealRetreat;
        }
        else
        {
            SwitchState(); 
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
        if (!isLeader && leader != null)
        {
            SyncStateWithLeader();
        }

    }

    private void SyncStateWithLeader()
    {
        if (leader.currentState == NPCStates.Attack)
        {
            
            if (currentState != NPCStates.Chase && currentState != NPCStates.Attack)
            {
                currentState = NPCStates.Chase;
               
                navMeshAgent.SetDestination(Player.position);
            }

    
            if (Vector3.Distance(transform.position, Player.position) <= AttackRange)
            {
                currentState = NPCStates.Attack;
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
        navMeshAgent.ResetPath(); 
        navMeshAgent.isStopped = true; 
        meshRenderer.material = AttackMaterial; // Change material to attack material
        transform.LookAt(Player); 

        Fire(); 

        // Check ifplayer is out of attack range
        if (Vector3.Distance(transform.position, Player.position) > AttackRange)
        {
            navMeshAgent.isStopped = false; // Allow NPC to move again
            currentState = NPCStates.Chase;
        }
    }

    private void Chase()
    {
        meshRenderer.material = ChaseMaterial; 
        navMeshAgent.SetDestination(Player.position); 

       
        if (Vector3.Distance(transform.position, Player.position) <= AttackRange)
        {
            currentState = NPCStates.Attack;
        }

        else if (Vector3.Distance(transform.position, Player.position) > ChaseRange)
        {
            navMeshAgent.ResetPath(); 
            currentState = NPCStates.Patrol;
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, Player.position) < ChaseRange)
        {
            currentState = NPCStates.Chase;
            return; 
        }

        
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            meshRenderer.material = PatrolMaterial; 

 
            nextPatrolPoint = (nextPatrolPoint + 1) % PatrolPoints.Length;
            navMeshAgent.SetDestination(PatrolPoints[nextPatrolPoint]);
        }
    }

    private void Retreat()
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

       
        navMeshAgent.isStopped = false; 
        navMeshAgent.SetDestination(PatrolPoints[farthestPointIndex]);
        meshRenderer.material = RetreatMaterial; 
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            currentState = NPCStates.Patrol;
            meshRenderer.material = PatrolMaterial; 
            navMeshAgent.SetDestination(PatrolPoints[nextPatrolPoint]); 
        }

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

       
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(PatrolPoints[farthestPointIndex]);
        meshRenderer.material = RetreatMaterial; 

       
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            if (health < 100)
            {
                health++; 
            }
            else
            {
                currentState = NPCStates.Patrol; 
                meshRenderer.material = PatrolMaterial; 
            }
        }
    }
    private void UpdateStateText()
    {
        if (stateText != null)
        {
            stateText.text = $" State: {currentState}";
            Debug.Log("Updated Text: " + stateText.text); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Seeker")) 
        {
            currentState = NPCStates.Retreat; 
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
    void TakeDamage(int baseDamage)
    {
        int actualDamage = CalculateActualDamage(baseDamage);
        health -= actualDamage;

        if (healthText != null)
        {
            healthText.text = "HP " + health;
        }

        Debug.Log("Damage Taken: " + actualDamage);
        Debug.Log("New Health: " + health);
    }
    int CalculateActualDamage(int baseDamage)
    {
        float probabilityFactor = UnityEngine.Random.Range(0f, 1f);
        switch (currentState)
        {
            case NPCStates.Retreat:
            case NPCStates.HealRetreat:
                // Less likely to take full damage in retreat states
                return probabilityFactor < 0.5f ? baseDamage / 2 : baseDamage;
            case NPCStates.Attack:
                // More likely to take full damage in attack state
                return probabilityFactor < 0.8f ? baseDamage : baseDamage / 2;
            default:
                
                return baseDamage;
        }
    }
}


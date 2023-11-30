using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class NPC_Melee : MonoBehaviour
{
    #region Variables
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI healthText;
    public string npcID = "NPC_Melee";
    public enum NPCStates
    {
        Patrol,
        Chase,
        Attack
    }

    [SerializeField] private Vector3[] patrolPoints;
    [SerializeField] private Transform player;
    [SerializeField] private Material patrolMaterial;
    [SerializeField] private Material chaseMaterial;
    [SerializeField] private Material attackMaterial;
    [SerializeField] private float chaseRange = 7f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int meleeDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float normalSpeed = 3.5f; 
    [SerializeField] private float chaseSpeed = 5.5f;

    private NPCStates currentState = NPCStates.Patrol;
    private NavMeshAgent navMeshAgent;
    private MeshRenderer meshRenderer;
    private int nextPatrolPoint = 0;
    private float lastAttackTime = 0;
    public int health = 100;
    #endregion
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(patrolPoints[nextPatrolPoint]);
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        UpdateStateText();

        SwitchState();
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
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, player.position) < chaseRange)
        {
            currentState = NPCStates.Chase;
            return;
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            meshRenderer.material = patrolMaterial;
            nextPatrolPoint = (nextPatrolPoint + 1) % patrolPoints.Length;
            navMeshAgent.SetDestination(patrolPoints[nextPatrolPoint]);
            navMeshAgent.speed = normalSpeed;
        }
    }

    private void Chase()
    {
        navMeshAgent.speed = chaseSpeed; // Increase speed when chasing
        meshRenderer.material = chaseMaterial;
        navMeshAgent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = NPCStates.Attack;
        }
        else if (Vector3.Distance(transform.position, player.position) > chaseRange)
        {
            navMeshAgent.ResetPath();
            currentState = NPCStates.Patrol;
            navMeshAgent.speed = normalSpeed; // Reset speed back to normal
        }
    }

    private void Attack()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        meshRenderer.material = attackMaterial;
        transform.LookAt(player);

        if (Vector3.Distance(transform.position, player.position) <= attackRange
            && Time.time > lastAttackTime + attackCooldown)
        {
            player.GetComponent<AgentController>().TakeDamage(meleeDamage);
            lastAttackTime = Time.time;
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            navMeshAgent.isStopped = false;
            currentState = NPCStates.Chase;
        }
    }

    private void UpdateStateText()
    {
        if (stateText != null)
        {
            stateText.text = $"{npcID} State: {currentState}";
        }
        if (healthText != null)
        {
            healthText.text = "HP " + health;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class NPCBIG : MonoBehaviour
{
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI healthText;
    public string npcID = "NPC";
    public enum NPCStates
    {
        Chase,
        Attack,      
    }

    [SerializeField] Transform Player;
    [SerializeField] Bullet Bullet;
    [SerializeField] Material AttackMaterial;
    [SerializeField] float ChaseRange = 15f; // Increased chase range
    [SerializeField] float AttackRange = 4f;
    [SerializeField] float FireRate = 0.5f; // Faster attack rate

    NPCStates currentState = NPCStates.Chase;
    NavMeshAgent navMeshAgent;
    MeshRenderer meshRenderer;

    float nextFire;
    public Transform bulletPosition;
    public GameObject bulletPrefab;
    public int health = 150;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        SwitchState();

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
        if (currentState == NPCStates.Attack)
        {
            Attack();
        }
        else
        {
            Chase();
        }
    }

    private void Attack()
    {
        navMeshAgent.isStopped = true;
        meshRenderer.material = AttackMaterial;
        transform.LookAt(Player);
        Fire();

        if (Vector3.Distance(transform.position, Player.position) > AttackRange)
        {
            currentState = NPCStates.Chase;
        }
    }

    private void Chase()
    {
        meshRenderer.material = AttackMaterial; // Using attack material for visual consistency
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(Player.position);

        if (Vector3.Distance(transform.position, Player.position) <= AttackRange)
        {
            currentState = NPCStates.Attack;
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
        health -= damage;
        if (healthText != null)
        {
            healthText.text = "HP " + health;
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AgentController : MonoBehaviour
{
    public float movementSpeed = 10f;
    Vector3 movementDirection;

    NavMeshAgent agent;

    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletPosition;
    float nextFire;

    [HideInInspector]
    public int health = 100;

    public Slider healthBar;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }
    void Update()
    {
        HandleMovementInput();
        if (health <= 0)
        {
            Respawn();
        }
    }
    private void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
            Vector3 movementDirection = movement.normalized;

            transform.LookAt(transform.position + movementDirection);
            agent.Move(movement * movementSpeed * Time.deltaTime);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
            Fire();
    }




    void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);

            bullet.GetComponent<Bullet>()?.InitializeBullet(transform.rotation * Vector3.forward);

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

    public void TakeDamage(int damage)
    {
        Debug.Log("Damage Taken: " + damage);
        health -= damage;
        Debug.Log("New Health: " + health);
        healthBar.value = health / 100f;

        if (health <= 0)
        {
            Respawn();
        }
    }
    void Respawn()
    {
        // Respawn logic here. This could be as simple as resetting the health
        // and placing the character back at a start position.
        health = 100;
        healthBar.value = 1;
        //transform.position = /* Your respawn position here */
        // You might also want to reset other states or properties.
    }

}

    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 15f;
    Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

    }
    public void InitializeBullet(Vector3 originalDirection)
    {
        Debug.Log("Bullet Direction: " + originalDirection); // Debugging line
        transform.forward = originalDirection;
        rigidBody.velocity = transform.forward * Speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}


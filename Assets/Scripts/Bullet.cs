using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 100f;
    Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

    }
    public void InitializeBullet(Vector3 originalDirection)
    {
        print(originalDirection);
        transform.forward = originalDirection;
        rigidBody.velocity = transform.forward * Speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}

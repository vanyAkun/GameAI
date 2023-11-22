using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100f;
    private Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        // Destroy the bullet after 2 seconds if it hasn't hit the player
        Destroy(gameObject, 2f);
    }

    public void InitializeBullet(Vector3 originalDirection)
    {
        print(originalDirection);
        transform.forward = originalDirection;
        rigidBody.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet has collided with the player
        if (collision.gameObject.tag == "Seeker")
        {
            // Perform any actions as a result of the bullet hitting the player
            // ...

            // Destroy the bullet immediately on collision with the player
            Destroy(gameObject);
        }
        // If it collides with anything else, it will also get destroyed because of the Destroy call in Awake.
        // No further action needed here.
    }
}

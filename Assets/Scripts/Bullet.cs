using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody rigidBody;
    public int damage = 10;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        if (rigidBody != null)
            print("Rigidbody is found!");
        else
            print("Rigidbody isn't found!");
    }

    public void InitializeBullet(Vector3 originalDirection)
    {
        print(originalDirection);
        transform.forward = originalDirection;
        rigidBody.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Destroy(gameObject);
    }
}

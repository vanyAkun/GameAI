using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletPosition;
    float nextFire;
    public AudioClip playerShootingAudio;
    public GameObject bulletFiringEffect;

    //[HideInInspector]
    public int health = 100;

    public Slider healthBar;

 



    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player")) 
        { 
            transform.LookAt(other.transform);
            Fire();
        }
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
            //TakeDamage(bullet.damage);
        }
    }

    void TakeDamage(int damage) 
    {
        health -= damage;

        healthBar.value = health / 100f;

    }


}

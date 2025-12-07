using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaptorDamage : MonoBehaviour
{
    public int damage; // Damage the bullet deals
    public ParticleSystem particleEffect;

    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the bullet hits an player
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Deal damage to the enemy
            }
            Instantiate(particleEffect, transform.position, Quaternion.identity);

        }
    }
}

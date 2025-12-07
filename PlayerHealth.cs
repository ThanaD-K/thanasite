using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth; // Maximum health
    public float currentHealth;
    public Image healthBar; // Reference to the health bar fill image
    public float lerpSpeed = 5f; // Speed at which the health bar fills
    private bool isDead = false; // Flag to check if enemy is dead

    void Start()
    {
        currentHealth = maxHealth; // Set health to max at the start
    }

    void Update()
    {
        // Smoothly transition health bar's fillAmount value
        if (!isDead) // Only lerp while alive
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, Time.deltaTime * lerpSpeed);
        }
        else
        {
            // Continue to transition even if the enemy is dead, just set fillAmount to 0
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, 0f, Time.deltaTime * lerpSpeed);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current HP: {currentHealth}");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true; 
        Debug.Log("Player died!");
        Destroy(gameObject, 1f); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Tutorialhealth : MonoBehaviour
{
    public float maxHealth; // Maximum health
    public float currentHealth;
    public Image healthBar; // Reference to the health bar fill image
    public float lerpSpeed = 5f; // Speed at which the health bar fills

    private int currentPhase = 1; // Current phase (1 = default, 2 = second phase, etc.)
    public ParticleSystem DiedEffect;

    public TutorialSkillManager skillManager;  // Reference to the SkillManager
    public SpellName skillNameUI;
    
    void Start()
    {
        currentHealth = maxHealth; // Set health to max at the start
        currentPhase = 1;
        OnPhaseChange();
    }

    void Update()
    {
        // Smoothly transition health bar's fillAmount value
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, Time.deltaTime * lerpSpeed);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Current HP: {currentHealth}");

        CheckPhase(); // Check if the phase should change

        if (currentHealth <= 0)
        {
            Die();

        }
    }

    void CheckPhase()
    {

        if (currentHealth <= 100 && currentPhase < 3)
        {
            currentPhase = 3;
            OnPhaseChange();
        }
        else if (currentHealth <= 200 && currentPhase < 2)
        {
            currentPhase = 2;
            OnPhaseChange();
        }
    }


    void OnPhaseChange()
    {
        Debug.Log($"Phase changed to {currentPhase}!");

        // Stop the current attack skill before changing phase
        if (skillManager != null)
        {
            skillManager.StopAttacking();  // Stop the current skill 
        }

        // Example skill names for each phase
        string skillName = currentPhase switch
        {
            1 => "Try dodge the bullet! and shoot by clicking",
            2 => "Try use spell to block or counter pressing E or R",
            3 => "Try use ultimate spell pressing F (Remember you can only activate 1 time each stage",
            _ => "Unknown Skill"
        };

        // Display the skill name
        skillNameUI.ShowSkillName(skillName);
        // Add behavior here for phase changes, e.g., change attack patterns
        skillManager.ActivateSkill(currentPhase);  // Activate the skill for the new phase
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        if (skillNameUI != null)
        {
            skillNameUI.HideSkillName();
        }
        if (skillManager != null)
        {
            skillManager.StopAttacking();  // Stop the skill when the enemy dies
        }
        StartCoroutine(DelayedEffect());
        Destroy(gameObject, 1); // Remove enemy from the game
        
    }
    IEnumerator DelayedEffect()
    {
        // Wait for 1 second (same as the delay for enemy destruction)
        yield return new WaitForSeconds(0.9f);
        Instantiate(DiedEffect, transform.position, Quaternion.identity);
        // Spawn the effect

    }
    

    public int GetCurrentPhase()
    {
        return currentPhase;
    }
}

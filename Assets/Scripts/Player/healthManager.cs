using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthManager : MonoBehaviour
{
    [SerializeField] GameObject heroEntity;
    [SerializeField] float MaxHealth;
    private float health;
    [SerializeField] Slider healthSlider;

    [SerializeField] GameObject playerSprite;
    [SerializeField] GameObject crossbowSprite;

    [SerializeField] GameObject deathScreen;
    [SerializeField] Rigidbody2D rb2d;
    private void Awake()
    {
        health = MaxHealth;
        healthSlider.value = health/MaxHealth;
    }

    private void Start()
    {
        playerSprite.SetActive(true);
        crossbowSprite.SetActive(true);
        deathScreen.SetActive(false);
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthSlider.value = health/MaxHealth;
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            Debug.Log("Vous êtes mort !");
            playerSprite.SetActive(false);
            crossbowSprite.SetActive(false);
            deathScreen.SetActive(true);
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
    
}

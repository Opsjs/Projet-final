using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]protected Rigidbody2D rb;
    public float health = 40f;

    [SerializeField] protected Transform Player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        CheckHealth();
        
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            Debug.Log("Enemy killed");
            GameObject.Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone"))
        {
            Destroy(gameObject);
        }
    }
}

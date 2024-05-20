using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float damage = 15f;
    [SerializeField] float LifeTime = 10f;
    [SerializeField] float LifeInit;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LifeInit = Time.time;
    }
    private void FixedUpdate()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (Time.time > LifeTime + LifeInit)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("EnemyArcher"))
        {
            collision.collider.GetComponent<ArcherEnemy>().health -= damage;
        }
        if (collision.collider.CompareTag("EnemyKnight"))
        {
            collision.collider.GetComponent<KnightEnemy>().health -= damage;
        }
        if (collision.collider.CompareTag("Breakable"))
        {
            Destroy(collision.collider.gameObject);
        }
        if (!collision.collider.CompareTag("Arrow") && !collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

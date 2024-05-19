using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    private HeroCapacities capacities;
    [SerializeField] float damage = 15f;
    [SerializeField] float LifeTime = 10f;
    [SerializeField] float LifeInit;
    [SerializeField] float speed;

    private void Start()
    {
        LifeInit = Time.time;
        capacities = FindObjectOfType<HeroCapacities>();
        if (capacities == null)
        {
            Debug.LogError("Damage Multiplier not found");
        }
    }
    private void FixedUpdate()
    {
        /*float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/
        transform.Translate(speed, 0, 0);
        if (Time.time > LifeTime + LifeInit)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        healthManager playerHealthManager = collision.collider.GetComponent<healthManager>();
        if (playerHealthManager != null)
        {
            Debug.Log("Vous êtes touché !");
            playerHealthManager.health -= damage * capacities.damageMultiplier;
        }
        if (!collision.collider.CompareTag("Arrow"))
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : MonoBehaviour
{
    public float health;
    [SerializeField] float range;
    [SerializeField] float _horizontalSpeed;
    [SerializeField] Transform player;
    [SerializeField] EnemyBowOrientation enemyBowOrientation;
    [SerializeField] GameObject arrow;
    [SerializeField] float arrowSpeed;
    [SerializeField] Transform shootingPoint;
    [SerializeField] float shootCooldown;
    private float LastTimeShot;
    private void Start()
    {
    }
    private void Update()
    {
        CheckHealth();
        if (Time.time - LastTimeShot > shootCooldown && CheckRange())
        {

            Shoot();
            LastTimeShot = Time.time;
        }
    }

    private bool CheckRange()
    {
        if ((player.position.x - transform.position.x) < range && -range < (player.position.x - transform.position.x))
        {
            return true;
        }
        return false;
    }
    private void Shoot()
    {   
        float direction_x = player.position.x - shootingPoint.position.x;
        float direction_y = player.position.y - shootingPoint.position.y;
        float angle = Mathf.Atan2(direction_y, direction_x) * Mathf.Rad2Deg;
        GameObject newArrow = Instantiate(arrow, shootingPoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
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

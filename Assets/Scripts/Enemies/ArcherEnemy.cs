using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : Enemy
{
    [SerializeField] float offset = 8.75f;
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
        if (Time.time - LastTimeShot > shootCooldown)
        {
            Shoot();
            LastTimeShot = Time.time;
        }
    }
    private void Shoot()
    {   
        float direction_x = player.position.x - shootingPoint.position.x;
        float direction_y = player.position.y - shootingPoint.position.y;
        float angle = Mathf.Atan2(direction_y, direction_x) * Mathf.Rad2Deg;
        GameObject newArrow = Instantiate(arrow, shootingPoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
    }

}

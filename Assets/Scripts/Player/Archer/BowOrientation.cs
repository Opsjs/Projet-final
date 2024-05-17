using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowOrientation : MonoBehaviour
{
    [SerializeField] Camera _camera;

    [SerializeField] GameObject enemy;
    [SerializeField] GameObject LightArrow;
    [SerializeField] GameObject HeavyArrow;
    [SerializeField] float LightAttackLaunchForce;
    [SerializeField] float HeavyAttackLaunchForce;

    public Transform shotPoint;
    [SerializeField] float LightAttackCooldown;
    [SerializeField] float LightAttackLastShot;

    [SerializeField] float HeavyAttackCooldown;
    [SerializeField] float HeavyAttackLastShot;

    private void FixedUpdate()
    {
        Vector2 bowPosition = transform.position;
        Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - bowPosition;
        transform.up = direction;

        if (Input.GetKey(KeyCode.Q))
        {
            if (Time.time - LightAttackLastShot >= LightAttackCooldown)
            {
                Shoot();
                LightAttackLastShot = Time.time;
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (Time.time - HeavyAttackLastShot >= HeavyAttackCooldown)
            {
                HeavyAttackArcher();
                HeavyAttackLastShot = Time.time;
            }
        }
        if (Input.GetKey(KeyCode.Tab))
        {
            GameObject newEnemy = Instantiate(enemy, new Vector3(transform.position.x + 3, transform.position.y, transform.position.z), transform.rotation);
        }
    }
    public void Shoot()
    {
        GameObject newArrow = Instantiate(LightArrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = transform.up * LightAttackLaunchForce;
    }

    public void HeavyAttackArcher()
    {
        GameObject newArrow = Instantiate(HeavyArrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = transform.up * HeavyAttackLaunchForce;

    }
}

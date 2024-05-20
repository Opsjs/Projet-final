using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightEnemy : MonoBehaviour
{
    [SerializeField] HeroCapacities HeroCapacities;
    public float health;
    [SerializeField] float detection;
    [SerializeField] float range;
    [SerializeField] float damage;
    [SerializeField] healthManager healthManager;
    [SerializeField] float cooldown;
    private float LastUse;
    [Header("Wall")]
    [SerializeField] private WallDetector _wallDetector;
    public bool IsTouchingWallLeft { get; private set; } = false;
    public bool IsTouchingWallRight { get; private set; } = false;

    private void Update()
    {
        CheckHealth();
        _ApplyWallDetection();
        if (IsTouchingWallLeft || IsTouchingWallRight)
        {
            if (Time.time - LastUse > cooldown)
            {
                Debug.Log("Vous êtes touché !");
                healthManager.health -= damage * HeroCapacities.damageMultiplier;
                LastUse = Time.time;
            }
        }
    }
    private void _ApplyWallDetection()
    {
        IsTouchingWallLeft = _wallDetector.DetectLeftWallNearby();
        IsTouchingWallRight = _wallDetector.DetectRightWallNearby();
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

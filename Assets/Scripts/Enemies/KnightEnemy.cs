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

    [Header("Movement")]
    [SerializeField] Transform player;
    [SerializeField] float speed;
    private Rigidbody2D rb2d;
    private float _verticalSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float DetectionRange;
    [SerializeField] HeroEntity Hero;


    [SerializeField] float knockbackCounter;
    public float knockbackTotalTime;
    public bool knocked;
    private enum directions
    {
        Left,
        Right,
    }
    private directions playerDirection;

    private void Start()
    {
                rb2d = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        if (knockbackCounter <= 0)
        {
            CheckPlayerPosition();
            Debug.Log("moving");
            if (rb2d.velocity.x > maxSpeed)
            {
                rb2d.velocity = new Vector2(maxSpeed, rb2d.velocity.y);
            }
            if (Mathf.Abs(Vector2.Distance(player.transform.position, transform.position)) < DetectionRange)
            {
                Move();
            }
            knocked = false;
        }
        else
        {
            if (playerDirection == directions.Right && knocked)
            {
                rb2d.velocity = new Vector2(-HeroCapacities.KnightHeavyAttackKnockback, 0);
            }
            else if (playerDirection == directions.Left && knocked)
            {
                rb2d.velocity = new Vector2(HeroCapacities.KnightHeavyAttackKnockback, 0);
            }
            knockbackCounter -= Time.fixedDeltaTime;
        }
        CheckHealth();
        _ApplyWallDetection();
        if (IsTouchingWallLeft || IsTouchingWallRight)
        {
            if (Time.time - LastUse > cooldown)
            {
                Debug.Log("Vous êtes touché !");
                knockbackCounter = knockbackTotalTime;
                healthManager.TakeDamage(damage * HeroCapacities.damageMultiplier);
                LastUse = Time.time;
            }
        }
    }
    private void Move()
    {
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos = player.transform.position;
        float distance = Vector2.Distance(playerPos, enemyPos);
        if (playerDirection == directions.Left && Hero.IsTouchingGround)
        {
            rb2d.velocity = new Vector2(-distance, rb2d.velocity.y).normalized * speed;
        }
        else if (playerDirection == directions.Right && Hero.IsTouchingGround)
        {
            rb2d.velocity = new Vector2(distance, rb2d.velocity.y).normalized * speed;
        }
    }
    private void CheckPlayerPosition()
    {
        if (player.position.x > transform.position.x)
        {
            playerDirection = directions.Right;
        }
        if (player.position.x < transform.position.x)
        {
            playerDirection = directions.Left;
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

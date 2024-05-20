using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float speed;
    private Rigidbody2D rb2d;
    private float _verticalSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float DetectionRange;
    [SerializeField] HeroEntity Hero;

    [SerializeField] HeroCapacities HeroCapacities;
    [SerializeField] float knockbackCounter;
    [SerializeField] float knockbackTotalTime;
    public bool knocked;
    private enum directions
    {
        Left,
        Right,
    }
    private directions playerDirection;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Vector3 start = transform.position;
        Vector3 end = player.transform.position;
        //StartCoroutine(MoveOverTime(start, end, 5f)); // Déplacer sur 2 secondes
    }

    private void FixedUpdate()
    {

/*        if (knockbackCounter <= 0 )
        {*/
            CheckPlayerPosition();
            Debug.Log("moving");
            if (rb2d.velocity.x > maxSpeed)
            {
                rb2d.velocity = new Vector2 (maxSpeed, rb2d.velocity.y);
            }
            if (Mathf.Abs(Vector2.Distance(player.transform.position, transform.position)) < DetectionRange)
            {
                Move();
            }
/*            
        } else
        {
            if (playerDirection == directions.Right && knocked)
            {
                rb2d.velocity = new Vector2(-HeroCapacities.KnightHeavyAttackKnockback, 0);
            } else if (playerDirection == directions.Left && knocked)
            {
                rb2d.velocity = new Vector2(HeroCapacities.KnightHeavyAttackKnockback, 0);
            }
            knockbackCounter -= Time.fixedDeltaTime;
        }*/
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



    private void Move()
    {
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos = player.transform.position;
        float distance = Vector2.Distance(playerPos, enemyPos);
        if (playerDirection == directions.Left && Hero.IsTouchingGround)
        {
            rb2d.velocity = new Vector2(-distance, rb2d.velocity.y).normalized * speed;
        } else if (playerDirection == directions.Right && Hero.IsTouchingGround) 
        {
            rb2d.velocity = new Vector2 (distance, rb2d.velocity.y).normalized * speed;
        }
    }
}

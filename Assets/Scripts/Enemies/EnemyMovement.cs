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
        CheckPlayerPosition();
        if (rb2d.velocity.x > maxSpeed)
        {
            rb2d.velocity = new Vector2 (maxSpeed, rb2d.velocity.y);
        }
        if (Mathf.Abs(Vector2.Distance(player.transform.position, transform.position)) < DetectionRange)
        {
            Move();
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



    private void Move()
    {
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos = player.transform.position;
        float distance = Vector2.Distance(playerPos, enemyPos);
        float velocityX = distance * speed;
        if (playerDirection == directions.Left && Hero.IsTouchingGround)
        {
            rb2d.velocity = new Vector2(-velocityX, rb2d.velocity.y);
        } else if (playerDirection == directions.Right && Hero.IsTouchingGround) 
        {
            rb2d.velocity = new Vector2 (velocityX, rb2d.velocity.y);
        }
    }

    private void _ApplyFallGravity(HeroFallSettings settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
        }
    }



    IEnumerator MoveOverTime(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            rb2d.velocity = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = end; // Assurer que la position finale est atteinte
    }

}

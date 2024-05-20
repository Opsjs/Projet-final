using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBowOrientation : MonoBehaviour
{
    public Transform player;
    public float offsetY;
    private void FixedUpdate()
    {
        Vector2 bowPosition = transform.position;
        Vector2 playerPosition = new Vector2(player.position.x, player.position.y + offsetY);
        Vector2 direction = playerPosition - bowPosition;
        transform.up = direction;
    }
}

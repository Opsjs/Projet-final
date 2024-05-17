using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightEnemy : Enemy
{
    [SerializeField] float range;
    [SerializeField] float damage;
    [SerializeField] Transform transform;
    [SerializeField] healthManager healthManager;
    [SerializeField] float cooldown;
    private float LastUse;
    [Header("Wall")]
    [SerializeField] private WallDetector _wallDetector;
    public bool IsTouchingWallLeft { get; private set; } = false;
    public bool IsTouchingWallRight { get; private set; } = false;

    private void Update()
    {
        /*RaycastHit2D hitLeft = CheckSurroundingsLeft();
        RaycastHit2D hitRight = CheckSurroundingsRight();
        if (hitLeft.collider.CompareTag("Player"))
        {
            hitLeft.collider.GetComponent<healthManager>().health -= damage;
        }
        if (hitRight.collider.CompareTag("Player"))
        {
            hitRight.collider.GetComponent<healthManager>().health -= damage;
        }*/
        _ApplyWallDetection();
        if (IsTouchingWallLeft)
        {
            if (Time.time - LastUse > cooldown)
            {
                Debug.Log("Vous êtes touché !");
                healthManager.health -= damage;
                LastUse = Time.time;
            }
        }
    }
    private RaycastHit2D CheckSurroundingsLeft()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), -transform.right, range);
        return hitLeft;
    }
    private RaycastHit2D CheckSurroundingsRight()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), transform.right, range);
        return hitRight;
    }
    private void _ApplyWallDetection()
    {
        IsTouchingWallLeft = _wallDetector.DetectLeftWallNearby();
        IsTouchingWallRight = _wallDetector.DetectRightWallNearby();
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnightEntity : HeroEntity
{

    #region Attacks

    protected bool isLightAttacking = false;
    protected bool isHeavyAttacking = false;
    protected bool isDefending = false;

    [Header("Knight Light attack")]
    public float damage;
    [SerializeField] private float range;
    [SerializeField] private float LightAttackCooldown = 2f;
    [SerializeField] private float LightAttackKnockback = 1f;
    private float LastLightAttack;

    #endregion
    protected override void LightAttack()
    {
        if (isLightAttacking && Time.time - LastLightAttack >= LightAttackCooldown)
        {
            Debug.Log("Attacking");
            RaycastHit2D hit = CheckEnemyPresence();
            if (hit.collider!=null && hit.collider.CompareTag("Enemy")){
                hit.collider.GetComponent<Enemy>().health -= damage;
                hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(LightAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                Debug.Log(transform.position.y - hit.collider.transform.position.y);
            }
            LastLightAttack = Time.time;
            isLightAttacking = false;
        }
    }

    private RaycastHit2D CheckEnemyPresence()
    {
        Debug.Log(_orientX);
        if (_orientVisualRoot.localScale.x == -1)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), -transform.right, range);
            Debug.Log("aaa");
            return hit;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), transform.right, range);
            Debug.Log("bbb");
            return hit;

        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        AttackInputCheck();
        LightAttack();
    }

    #region Attacks Setup

    private void AttackInputCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isLightAttacking = true;
            isHeavyAttacking = false;
            isDefending = false;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            isHeavyAttacking = true;
            isLightAttacking = false;
            isDefending = false;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            isDefending = true;
            isLightAttacking = false;
            isHeavyAttacking = false;
        }
    }


    #endregion
}

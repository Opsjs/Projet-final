using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroCapacities : MonoBehaviour
{
    [SerializeField] Camera _camera;
    private Vector2 mousePos;
    public int damageMultiplier = 1;
    
    public enum PlayerState
    {
        Archer,
        Knight
    }
    public PlayerState playerState = PlayerState.Archer;
    [SerializeField] HeroEntity _entity;


    private void FixedUpdate()
    {
        Debug.Log(damageMultiplier);
        mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            swapPlayerState();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            LightAttack();
        }
        if (Input.GetKey(KeyCode.E))
        {
            HeavyAttack();
        }
        if (Input.GetKey(KeyCode.X))
        {
            NonOffensiveCapacity();
        }
        if (playerState == PlayerState.Knight)
        {
            if (damageMultiplier == 0 && Time.time > LastShieldUse + immunityTime)
            {
                damageMultiplier = 1;
            }
        }
    }

    public void swapPlayerState()
    {
        if (playerState == PlayerState.Archer)
        {
            playerState = PlayerState.Knight;
            Bow.SetActive(false);
        } else
        {
            playerState = PlayerState.Archer;
            Bow.SetActive(true);
        }
    }

    [Header("Archer")]
    public Transform shotPoint;
    [SerializeField] GameObject Bow;
    #region Light Attack

    #region Archer Light Attack
    
    [Header("Archer Light Attack")]
    [SerializeField] GameObject LightArrow;
    [SerializeField] float ArcherLightAttackLaunchForce;
    [SerializeField] float ArcherLightAttackCooldown;
    private float LastArcherLightAttack;

    public void ArcherLightAttack()
    {
        if (Time.time - LastArcherLightAttack >= ArcherLightAttackCooldown)
        {
            Debug.Log("shooting");
            ShootLightAttack();
            LastArcherLightAttack = Time.time;
        }
    }

    public void ShootLightAttack()
    {
        GameObject newArrow = Instantiate(LightArrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = Bow.transform.up * ArcherLightAttackLaunchForce;
    }
    #endregion

    #region Knight Light Attack

    [Header("Knight Light Attack")]
    [SerializeField] float KnightLightAttackDamage;
    [SerializeField] float KnightLightAttackKnockback;
    [SerializeField] float KnightLightAttackRange;
    [SerializeField] float KnightLightAttackCooldown;
    private float LastKnightLightAttack;
    public void KnightLightAttack()
    {
        if (Time.time - LastKnightLightAttack >= KnightLightAttackCooldown)
        {
            Debug.Log("Attacking");
            RaycastHit2D hit = CheckEnemyPresenceLightAttack();
            if (hit.collider != null && hit.collider.CompareTag("EnemyKnight"))
            {
                hit.collider.GetComponent<KnightEnemy>().health -= KnightLightAttackDamage;
                if (_entity.OrientVisualRoot.localScale.x == 1)
                {
                    hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(KnightLightAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                } else
                {
                    hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(-KnightLightAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                }
            } else if (hit.collider != null && hit.collider.CompareTag("EnemyArcher"))
            {
                hit.collider.GetComponent<ArcherEnemy>().health -= KnightLightAttackDamage;
                if (_entity.OrientVisualRoot.localScale.x == 1)
                {
                    hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(KnightLightAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                }
                else
                {
                    hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(-KnightLightAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                }
            }
            LastKnightLightAttack = Time.time;

        }
    }
    private RaycastHit2D CheckEnemyPresenceLightAttack()
    {
        if (_entity.OrientVisualRoot.localScale.x == -1)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), -transform.right, KnightLightAttackRange);
            return hit;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), transform.right, KnightLightAttackRange);
            return hit;

        }
    }
    private void ApplyKnockBack(float knockback)
    {

    }
    #endregion
    public void LightAttack()
    {
        switch (playerState)
        {
            case PlayerState.Archer:
                ArcherLightAttack(); 
                break;
            case PlayerState.Knight:
                KnightLightAttack();
                break;
        }
    }
    #endregion

    #region Heavy Attacks

    #region Archer Heavy Attack
    [Header("Archer Heavy Attack")]
    [SerializeField] GameObject HeavyArrow;
    [SerializeField] float ArcherHeavyAttackLaunchForce;
    [SerializeField] float ArcherHeavyAttackCooldown;
    private float LastArcherHeavyAttack;
    public void ArcherHeavyAttack()
    {
        if (Time.time - LastArcherHeavyAttack >= ArcherHeavyAttackCooldown)
        {
            Debug.Log("Shooting Heavy Attack");
            ShootHeavyAttack();
            LastArcherHeavyAttack = Time.time;
        }
    }

    public void ShootHeavyAttack()
    {
        GameObject newArrow = Instantiate(HeavyArrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = Bow.transform.up * ArcherHeavyAttackLaunchForce;
    }

    #endregion

    #region Knight Heavy Attack

    [Header("Knight Heavy Attack")]
    [SerializeField] float KnightHeavyAttackDamage;
    [SerializeField] float KnightHeavyAttackKnockback;
    [SerializeField] float KnightHeavyAttackRange;
    [SerializeField] float KnightHeavyAttackCooldown;
    private float LastKnightHeavyAttack;

    public void KnightHeavyAttack()
    {
        if (Time.time - LastKnightHeavyAttack >= KnightHeavyAttackCooldown)
        {
            Debug.Log("Attacking with Heavy Attack");
            RaycastHit2D hit = CheckEnemyPresenceHeavyAttack();
            if (hit.collider != null && hit.collider.CompareTag("EnemyKnight"))
            {
                hit.collider.GetComponent<KnightEnemy>().health -= KnightHeavyAttackDamage;
                if (_entity.OrientVisualRoot.localScale.x == 1)
                {
                    hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(KnightHeavyAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                }
                else
                {
                    hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(-KnightHeavyAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                }
            }
            else if (hit.collider != null && hit.collider.CompareTag("EnemyArcher"))
            {
                hit.collider.GetComponent<ArcherEnemy>().health -= KnightHeavyAttackDamage;
                if (_entity.OrientVisualRoot.localScale.x == 1)
                {
                    hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(KnightHeavyAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                }
                else
                {
                    hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(-KnightHeavyAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
                }
            }
            LastKnightHeavyAttack = Time.time;

        }
    }
    private RaycastHit2D CheckEnemyPresenceHeavyAttack()
    {
        if (_entity.OrientVisualRoot.localScale.x == -1)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z), -transform.right, KnightLightAttackRange);
            return hit;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), transform.right, KnightLightAttackRange);
            return hit;

        }
    }


        #endregion
        public void HeavyAttack()
    {
        switch (playerState)
        {
            case PlayerState.Archer:
                ArcherHeavyAttack();
                break;
            case PlayerState.Knight:
                KnightHeavyAttack();
                break;
        }
    }

    #endregion

    #region Non Offensive Capacities

    #region Knight Shield
    [SerializeField] float immunityTime;
    [SerializeField] float immunityCooldown;
    [SerializeField] float LastShieldUse;
    private float shieldStart;
    private void KnightShield()
    {
        if (Time.time - LastShieldUse > immunityCooldown)
        {
            damageMultiplier = 0;
            LastShieldUse = Time.time;
        }
    }

    #endregion



    private void NonOffensiveCapacity()
    {
        switch (playerState)
        {
            case PlayerState.Knight:
                shieldStart = Time.time;
                KnightShield();
                break;
        }
    }
    #endregion
}

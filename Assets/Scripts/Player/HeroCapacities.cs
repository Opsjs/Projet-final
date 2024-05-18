using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCapacities : MonoBehaviour
{
    public enum PlayerState
    {
        Archer,
        Knight
    }
    public PlayerState playerState = PlayerState.Archer;
    [SerializeField] HeroEntity _entity;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            swapPlayerState();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            LightAttack();
        }
        if (playerState == PlayerState.Archer)
        {

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

    #region Light Attack
    [Header("Archer")]
    public Transform shotPoint;
    [SerializeField] GameObject Bow;

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
            Shoot();
            LastArcherLightAttack = Time.time;
        }
    }

    public void Shoot()
    {
        GameObject newArrow = Instantiate(LightArrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = Bow.transform.up * ArcherLightAttackLaunchForce;
    }
    #endregion

    #region Knight Light Attack

    [Header("KnightLightAttack")]
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
            RaycastHit2D hit = CheckEnemyPresence();
            if (hit.collider != null && hit.collider.CompareTag("EnemyKnight"))
            {
                hit.collider.GetComponent<KnightEnemy>().health -= KnightLightAttackDamage;
                hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(KnightLightAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
            } else if (hit.collider != null && hit.collider.CompareTag("EnemyArcher"))
            {
                hit.collider.GetComponent<ArcherEnemy>().health -= KnightLightAttackDamage;
                hit.collider.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(KnightLightAttackKnockback, transform.position.y - hit.collider.transform.position.y), ForceMode2D.Impulse);
            }
            LastKnightLightAttack = Time.time;

        }
    }
    private RaycastHit2D CheckEnemyPresence()
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
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherEntity : HeroEntity
{
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    private Rigidbody2D rb;


    /*private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
            Debug.Log("bbb");
        }
    }
    IEnumerator Dash()
    {
        isDashing = true;
        float timer = 0f;
        Debug.Log("aaa");

        // Calculer la direction du dash (exemple : vers la droite)
        Vector3 dashDirection = transform.right;

        while (timer < dashDuration)
        {
            // Déplacer le joueur dans la direction du dash à la vitesse du dash
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // Réactiver les contrôles de mouvement normal
        isDashing = false;
    }*/

    #region dash
    [Header("Dash")]
    [SerializeField] private HeroJumpSettings _dashGroundSettings;
    [SerializeField] private HeroJumpSettings _dashAirSettings;
    [SerializeField] private HeroFallSettings _dashFallSettings;
    enum DashState
    {
        NotDashing,
        DashImpulsion,
        Falling,
    }
    private DashState _dashstate = DashState.NotDashing;
    private float _dashTimer = 0f;
    //private float _dashTime = 0f;
    public bool IsDashing => _dashstate != DashState.NotDashing;
    public bool IsDashImpulsing => _dashstate == DashState.DashImpulsion;
    public bool IsGroundDashMinDurationReached => _dashTimer >= _dashGroundSettings.jumpMinDuration;
    public bool IsAirDashMinDurationReached => _dashTimer >= _dashAirSettings.jumpMinDuration;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }
    protected override void Update()
    {
        base.Update();
        if (IsDashing)
        {
            Debug.Log("dash update ");
            _UpdateDash();
        }
        if (Input.GetKey(KeyCode.X))
        {
            Debug.Log("aaaaaaa");

            
        }
    }

    public void DashStart()
    {
        _dashstate = DashState.DashImpulsion;
        _dashTimer = 0f;
    }
    private void _UpdateDashStateImpulsion()
    {
        _dashTimer += Time.deltaTime;
        if (!IsTouchingGround)
        {
            if (_dashTimer < _dashAirSettings.jumpMaxDuration)
            {
                _horizontalSpeed = _dashAirSettings.jumpSpeed;
                _ResetVerticalSpeed();
            }
            else
            {
                _dashstate = DashState.Falling;
            }
        } else
        {
            if (_dashTimer < _dashGroundSettings.jumpMaxDuration)
            {
                _horizontalSpeed = _dashGroundSettings.jumpSpeed;
                _ResetVerticalSpeed();
            }
            else
            {
                _dashstate = DashState.Falling;
            }
        }
        
    }
    private void _ResetHorizontalSpeed()
    {
        _horizontalSpeed = 0;
    }
    private void _UpdateDashStateFalling()
    {
        if (!IsTouchingGround)
        {
            _ResetHorizontalSpeed();
            //_ApplyFallGravity(_dashFallSettings);
        } else
        {
            _ResetHorizontalSpeed();
            _dashstate = DashState.NotDashing;
        }
    }
    private void _UpdateDash()
    {
        switch (_dashstate)
        {
            case DashState.DashImpulsion:
                _UpdateDashStateImpulsion();
                break;
            case DashState.Falling:
                _UpdateDashStateFalling();
                break;
        }
    }

    public void StopDashImpulsion()
    {
        _horizontalSpeed= 0;
        _dashstate= DashState.Falling;
    }



    #endregion
}

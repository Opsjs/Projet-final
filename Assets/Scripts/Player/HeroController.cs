using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Entity")]
    [SerializeField] private HeroEntity _entity;
    private bool  _entityWasTouchingGround = false;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    [Header("Jump buffer")]
    [SerializeField] private float _jumpBufferDuration = 0.2f;
    private float _jumpBufferTimer = 0f;

    [Header("Coyote Time")]
    [SerializeField] private float _coyoteTimeDuration = 0.2f;
    private float _coyoteTimeCountdown = -1f;

    #region PlayerStatus

    [Header("Player Status")]
    [SerializeField] bool isArcher = true;

    [Header("Player Status Scripts")]
    [SerializeField] ArcherEntity archerEntity;
    [SerializeField] KnightEntity knightEntity;

    #endregion

    
    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"Jump Buffer Timer : {_jumpBufferTimer}");
        GUILayout.Label($"Coyote Time Countdown : {_coyoteTimeCountdown}");
        GUILayout.EndVertical();
    }
    private void Start()
    {
        _ResetJumpBuffer();
    }
    private void FixedUpdate()
    {
        if (isArcher)
        {
            archerEntity.enabled = true;
            knightEntity.enabled = false;
        } else
        {
            archerEntity.enabled = false;
            knightEntity.enabled = true;
        }
    }
    private void Update()
    {
        _UpdateJumpBuffer();
        #region Dash
        if (_GetInputDownDash())
        {
            archerEntity.DashStart();
        }
        #endregion
        if (!archerEntity.IsDashImpulsing)
        {
            _entity.SetMoveDirX(GetInputMoveX());
            
        }
        
        _entity.SetMoveDirX(GetInputMoveX());
        if (_EntityHasExitGround())
        {
            _ResetCoyoteTime();
        } else
        {
            _UpdateCoyoteTime();
        }
        if (_GetInputDownJump())
        {
            if ((_entity.IsTouchingGround || _IsCoyoteTimeActive()) && !_entity.IsJumping)
            {
                _entity._JumpStart();
            } else
            {
                _ResetJumpBuffer();
            }
        }
        if (_IsJumpBufferActive())
        {
            if ((_entity.IsTouchingGround || _IsCoyoteTimeActive()) && !_entity.IsJumping)
            {
                _entity._JumpStart();
            }
        }
        if (_entity.IsJumpImpulsing)
        {
            if (!_GetInputJump() && _entity.IsJumpMinDurationReached)
            {
                _entity.StopJumpImpulsion();
            }
        }
        _entityWasTouchingGround = _entity.IsTouchingGround;
    }

    private float GetInputMoveX()
    {
        float inputMoveX = 0f;
        if (Input.GetKey(KeyCode.A) )
        {
            inputMoveX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveX = 1f;
        }

        return inputMoveX;
    }
    private bool _GetInputJump()
    {
        return Input.GetKey(KeyCode.Space);
    }
    private bool _GetInputDownJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }



    private void _ResetJumpBuffer()
    {
        _jumpBufferTimer = 0f;
    }
    private bool _IsJumpBufferActive()
    {
        return _jumpBufferTimer < _jumpBufferDuration;
    }
    private void _UpdateJumpBuffer()
    {
        if (!_IsJumpBufferActive()) return;
        _jumpBufferTimer += Time.deltaTime;
    }
    private void _CancelJumpBuffer()
    {
        _jumpBufferTimer = _jumpBufferDuration;
    }

    private bool _IsCoyoteTimeActive()
    {
        return _coyoteTimeCountdown > 0f;
    }
    private void _UpdateCoyoteTime()
    {
        if (!_IsCoyoteTimeActive()) return;
        _coyoteTimeCountdown -= Time.deltaTime;
    }
    private void _ResetCoyoteTime()
    {
        _coyoteTimeCountdown = _coyoteTimeDuration;
    }
    private bool _EntityHasExitGround()
    {
        return _entityWasTouchingGround && !_entity.IsTouchingGround;
    }

    #region Dash
    private bool _GetInputDownDash()
    {
        return Input.GetKeyDown(KeyCode.X);
    }
    #endregion

}
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{
    [Header("Capacities")]
    [SerializeField] HeroCapacities capacities;
    [Header("Health")]
    [SerializeField] healthManager playerHealth;
    [Header("Physics")]
    [SerializeField] public Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [FormerlySerializedAs("_movementSettings")]
    [SerializeField] private HeroHorizontalMovementSettings _groundHorizontalMovementSettings;
    [SerializeField] private HeroHorizontalMovementSettings _airHorizontalMovementSettings;
    protected float _horizontalSpeed = 5f;
    private float _moveDirX = 0f;

    [Header("Vertical Movement")]
    private float _verticalSpeed = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSettings _fallsettings;

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;
    public GroundDetector groundDetector { get => _groundDetector; }
    public bool IsTouchingGround { get; private set; } = false;

    [Header("Wall")]
    [SerializeField] private WallDetector _wallDetector;
    public bool IsTouchingWallLeft { get; private set; } = false;
    public bool IsTouchingWallRight { get; private set; } = false;

    [Header("Jump")]
    [SerializeField] private HeroJumpSettings _jumpSettings;
    private bool isDoubleJumping = false;


    public float dashForce;

    public enum JumpState
    {
        NotJumping, 
        JumpImpulsion,
        Falling,
    }
    private JumpState _jumpstate = JumpState.NotJumping;
    public JumpState jumpState { get => _jumpstate; }
    private float _jumpTimer = 0f;
    private float _jumpTime = 0f;
    public bool IsJumping => _jumpstate != JumpState.NotJumping;
    public bool IsJumpImpulsing => _jumpstate == JumpState.JumpImpulsion;
    public bool IsJumpMinDurationReached => _jumpTimer >= _jumpSettings.jumpMinDuration;

    /*#region dash
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
    private float _dashTime = 0f;
    public bool IsDashing => _dashstate != DashState.NotDashing;
    public bool IsDashImpulsing => _dashstate == DashState.DashImpulsion;
    public bool IsGroundDashMinDurationReached => _dashTimer >= _dashGroundSettings.jumpMinDuration;
    public bool IsAirDashMinDurationReached => _dashTimer >= _dashAirSettings.jumpMinDuration;*/


    /*public void DashStart()
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
            _ApplyFallGravity(_dashFallSettings);
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



    #endregion*/

    #region Wall Reset Speed


    private void _ApplyWallDetection()
    {
        IsTouchingWallLeft = _wallDetector.DetectLeftWallNearby();
        IsTouchingWallRight = _wallDetector.DetectRightWallNearby();
    }
    /*private void _ResetDashIfTouchingWall()
    {

        if (IsTouchingWallLeft && _orientX == -1 && _dashstate != DashState.NotDashing || IsTouchingWallRight && _orientX == 1 && _dashstate != DashState.NotDashing)
        {
            _horizontalSpeed = 0;
            _dashstate = DashState.NotDashing;
        }
    }*/
    #endregion

    #region Wall Slide / Jump

    [Header("Wall Slide/Jump")]
    [SerializeField] private HeroFallSettings _wallSlideSettings;
    [SerializeField] private HeroJumpSettings _wallJumpSettings;



    #endregion



    [Header("Orientation")]
    [SerializeField] protected Transform _orientVisualRoot;
    public Transform OrientVisualRoot { get { return _orientVisualRoot; } }
    [SerializeField] protected float _orientX = 1f;
    //protected float OrientX { get=>_orientX;}
    
    #region Camera offset
    public float OrientX { get => _orientX;}
    #endregion

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    public float cooldown = 1.0f;

    //Camera Follow 
    private CameraFollowable _cameraFollowable;

    private Vector3 _respawnPoint;


    private void Awake()
    {
        _cameraFollowable = GetComponent<CameraFollowable>();
        _cameraFollowable.FollowPositionX = _rigidbody.position.x;
        _cameraFollowable.FollowPositionY = _rigidbody.position.y;
    }

    protected virtual void Start()
    {
        _respawnPoint = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone"))
        {
            playerHealth.TakeDamage(100000000);
        }
    }

    protected virtual void FixedUpdate()
    {
        _ApplyGroundDetection();

        //_ApplyWallDetection();
        
        _UpdateCameraFollowPosition();

        HeroHorizontalMovementSettings horizontalMovementSettings = _GetCurrentHorizontalMovementSettings();
        

        if (_AreOrientAndMovementOpposite())
        {
            _TurnBack(horizontalMovementSettings);
        } else
        {
            _UpdateHorizontalSpeed(horizontalMovementSettings);
            _ChangeOrientFromHorizontalMovement();
        }
        
        if (_jumpstate == JumpState.Falling && !isDoubleJumping)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                capacities.damageMultiplier = 0;
                isDoubleJumping = true;
                _jumpstate = JumpState.JumpImpulsion;
                _jumpTime = 0f;
            }
        }

        if (IsJumping)
        {
            _UpdateJump();
        }
        else
        {
            
            if (!IsTouchingGround)
            {
                _ApplyFallGravity(_fallsettings);
            }
            else
            {
                isDoubleJumping = false;
                capacities.damageMultiplier = 1;
                _ResetVerticalSpeed();
            }
        }
        
        /*if (IsTouchingWallLeft || IsTouchingWallRight)
        {
            _ApplyFallGravity(_wallSlideSettings);
        }*/

        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
        

    }

    public void SetMoveDirX(float dirX)
    {
        _moveDirX= dirX;
    }

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }
    
    protected virtual void Update()
    {
        _UpdateOrientVisual();
    }

    private void _UpdateOrientVisual()
    {
        /*switch (_orientX)
        {
            case -1:
                Vector3 newScale = _orientVisualRoot.localScale;
                newScale.x = -1;
                Debug.Log(newScale);
                _orientVisualRoot.localScale = newScale;
                break;
            case 1:
                Vector3 newScale2 = _orientVisualRoot.localScale;
                newScale2.x = 1;
                Debug.Log(newScale2);
                _orientVisualRoot.localScale = newScale2;
                break;
        }*/
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }

    private void _ChangeOrientFromHorizontalMovement()
    {
        if (_moveDirX == 0) return;
        _orientX = Mathf.Sign( _moveDirX );
    }

    private void _Accelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed += settings.acceleration * Time.deltaTime;
        if (_horizontalSpeed > settings.speedMax)
        {
            _horizontalSpeed= settings.speedMax;
        }
    }
    private void _Decelerate(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.deceleration* Time.deltaTime;
        if (_horizontalSpeed < 0) 
        { 
            _horizontalSpeed = 0;
        }
    }

    private void _UpdateHorizontalSpeed(HeroHorizontalMovementSettings settings)
    {
        if (_moveDirX != 0f)
        {
            _Accelerate(settings);
        } else
        {
            _Decelerate(settings);
        }
    }
    private void _TurnBack(HeroHorizontalMovementSettings settings)
    {
        _horizontalSpeed -= settings.turnBackFrictions * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0)
        {
            _horizontalSpeed = 0;
            _ChangeOrientFromHorizontalMovement();
        }
    }
    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    private void _ApplyFallGravity(HeroFallSettings settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
        }
    }
    
    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    private void _ApplyGroundDetection()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearby();
    }
    protected void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }
    #region SingleJump


    public void _JumpStart()
    {
        _jumpstate = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }
    private void _UpdateJumpStateImpulsion()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < _jumpSettings.jumpMaxDuration)
        {
            _verticalSpeed = _jumpSettings.jumpSpeed;
        }
        else
        {
            _jumpstate = JumpState.Falling;
        }
    }
    /*private void _UpdateJumpStateImpulsion(HeroJumpSettings settings)
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < settings.jumpMaxDuration)
        {
            _verticalSpeed = settings.jumpSpeed;
        }
        else
        {
            _jumpstate = JumpState.Falling;
        }
    }
*/
    private void _UpdateJumpStateFalling()
    {
        if (!IsTouchingGround || _verticalSpeed > 0)
        {
            _ApplyFallGravity(_fallsettings);
        }
        else
        {
            _ResetVerticalSpeed();
            _jumpstate = JumpState.NotJumping;
        }
    }

    private void _UpdateJump()
    {
        switch (_jumpstate)
        {
            case JumpState.JumpImpulsion:
                _UpdateJumpStateImpulsion();
                break;
            case JumpState.Falling:
                _UpdateJumpStateFalling();
                break;
        }
    }


    public void StopJumpImpulsion()
    {
        _jumpstate = JumpState.Falling;
    }
    #endregion



    private HeroHorizontalMovementSettings _GetCurrentHorizontalMovementSettings()
    {
        return IsTouchingGround ? _groundHorizontalMovementSettings : _airHorizontalMovementSettings;
    }


    protected virtual void LightAttack()
    {

    }
    protected virtual void HeavyAttack()
    {

    }
    protected virtual void Defense()
    {

    }

    private void _UpdateCameraFollowPosition()
    {

        _cameraFollowable.FollowPositionX = _rigidbody.position.x ;
        if (IsTouchingGround && !IsJumping)
        {
            _cameraFollowable.FollowPositionY = _rigidbody.position.y;
        }
    }

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        if (IsTouchingGround)
        {
            GUILayout.Label("On Ground");
        } else { 
            GUILayout.Label("In Air");
        }
        GUILayout.Label($"Left Wall = {IsTouchingWallLeft}");
        GUILayout.Label($"Right Wall = {IsTouchingWallRight}");
        GUILayout.Label($"JumpState = {_jumpstate}");

        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.EndVertical();
    }
}
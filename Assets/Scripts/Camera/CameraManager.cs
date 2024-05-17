using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Camera")]
    [SerializeField] private Camera _camera;

    [Header("Profile System")]
    [SerializeField] private CameraProfile _defaultCameraProfile;
    private CameraProfile _currentCameraProfile;

    //Transition
    private float _profileTransitionTimer = 0f;
    private float _profileTransitionDuration = 0f;
    private Vector3 _profileTransitionStartPosition;
    private float _profileTransitionStartSize;

    //Followable
    private Vector3 _profileLastFollowDestination;

    //Damping
    private Vector3 _dampedPosition;

    //Camera offset
    [SerializeField] public int FollowOffsetX;
    [SerializeField] private float FollowOffsetDamping;
    [SerializeField] private HeroEntity _entity;


    //AutoScroll
    [SerializeField] private float autoScrollSpeed = 2;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _InitToDefaultProfile();
    }

    private void Update()
    {
        Vector3 _nextPosition = _FindCameraNextPosition();
        _nextPosition = _ClampPositionIntoBounds(_nextPosition);
        _nextPosition = _ApplyDamping(_nextPosition);
        if (_IsPlayingProfileTransition())
        {
            _profileTransitionTimer += Time.deltaTime;
            Vector3 transitionPosition = _CalculateProfileTransitionCameraPosition(_nextPosition);
            _SetCameraPosition(transitionPosition);
            float transitionSize = _CalculateProfileTransitionCameraSize(_currentCameraProfile.CameraSize);
            _SetCameraSize(transitionSize);
        } else
        {
            _SetCameraPosition(_nextPosition);
            _SetCameraSize(_currentCameraProfile.CameraSize);
        }
        if (_currentCameraProfile.ProfileType == CameraProfileType.AutoScroll)
        {
            _SetCameraPosition(new Vector3(_currentCameraProfile.Position.x + autoScrollSpeed, _currentCameraProfile.Position.y, _currentCameraProfile.Position.z));
        }

    }

    private void _InitToDefaultProfile()
    {
        _currentCameraProfile = _defaultCameraProfile;
        _SetCameraPosition(_currentCameraProfile.Position);
        _SetCameraSize(_currentCameraProfile.CameraSize);
        _SetCameraDampedPosition(_FindCameraNextPosition());
    }

    private void _SetCameraPosition(Vector3 position)
    {
        Vector3 newCameraPosition = _camera.transform.position;
        newCameraPosition.x = position.x;
        newCameraPosition.y = position.y;
        _camera.transform.position = newCameraPosition;
    }
    private void _SetCameraSize(float size)
    {
        _camera.orthographicSize = size;
    }

    public void EnterProfile(CameraProfile cameraProfile, CameraProfileTransition transition = null)
    {
        _currentCameraProfile = cameraProfile;
        if (transition != null)
        {
            _PlayProfileTransition(transition);
        }
        _SetCameraDampedPosition(_FindCameraNextPosition());
    }
    public void exitProfile(CameraProfile cameraProfile, CameraProfileTransition transition = null)
    {
        if (_currentCameraProfile != cameraProfile) return;
        _currentCameraProfile = _defaultCameraProfile;
        if (transition != null)
        {
            _PlayProfileTransition(transition);
        }
        _SetCameraDampedPosition(_FindCameraNextPosition());
    }
    private void _PlayProfileTransition(CameraProfileTransition transition)
    {
        _profileTransitionStartPosition = _camera.transform.position;
        _profileTransitionStartSize = _camera.orthographicSize;
        _profileTransitionTimer = 0f;
        _profileTransitionDuration = transition.duration;
    }
    private bool _IsPlayingProfileTransition()
    {
        return _profileTransitionTimer < _profileTransitionDuration;
    }

    private float _CalculateProfileTransitionCameraSize(float endSize)
    {
        float percent = _profileTransitionTimer / _profileTransitionDuration;
        float startSize = _profileTransitionStartSize;
        return Mathf.Lerp(startSize, endSize, percent);
    }
    private Vector3 _CalculateProfileTransitionCameraPosition(Vector3 destination)
    {
        float percent = _profileTransitionTimer / _profileTransitionDuration;
        Vector3 origin = _profileTransitionStartPosition;
        return Vector3.Lerp(origin, destination, percent);
    }

    private Vector3 _FindCameraNextPosition()
    {
        if (_currentCameraProfile.ProfileType == CameraProfileType.FollowTargetOffset)
        {
            if (_currentCameraProfile.TargetToFollow != null)
            {
                if (_entity.OrientX == -1)
                {
                    CameraFollowable targetToFollow = _currentCameraProfile.TargetToFollow;
                    _profileLastFollowDestination.x = targetToFollow.FollowPositionX - FollowOffsetX;
                    _profileLastFollowDestination.y = targetToFollow.FollowPositionY;
                } else
                {
                    CameraFollowable targetToFollow = _currentCameraProfile.TargetToFollow;
                    _profileLastFollowDestination.x = targetToFollow.FollowPositionX + FollowOffsetX;
                    _profileLastFollowDestination.y = targetToFollow.FollowPositionY;
                }
                
                
                return _profileLastFollowDestination;
            }
        }
        else if (_currentCameraProfile.ProfileType == CameraProfileType.FollowTarget)
        {
            if(_currentCameraProfile.TargetToFollow != null)
            {
                CameraFollowable targetToFollow = _currentCameraProfile.TargetToFollow;
                _profileLastFollowDestination.x = targetToFollow.FollowPositionX;
                _profileLastFollowDestination.y = targetToFollow.FollowPositionY;

                return _profileLastFollowDestination;
            }
        }
        // Camera scroll auto
        else if (_currentCameraProfile.ProfileType == CameraProfileType.AutoScroll)
        {
            
            _profileLastFollowDestination.x += autoScrollSpeed;
            
            return _profileLastFollowDestination;
        }
        return _currentCameraProfile.Position;
    }

    private Vector3 _ApplyDamping(Vector3 position)
    {
        if (_currentCameraProfile.UseDampingHorizontally)
        {
            _dampedPosition.x = Mathf.Lerp(_dampedPosition.x, position.x, _currentCameraProfile.HorizontalDampingFactor * Time.deltaTime);
        } else
        {
            _dampedPosition.x = position.x;
        }
        if (_currentCameraProfile.UseDampingVertically)
        {
            _dampedPosition.y = Mathf.Lerp(_dampedPosition.y, position.y, _currentCameraProfile.VerticalDampingFactor * Time.deltaTime);
        } else
        {
            _dampedPosition.y = position.y;
        }
        return _dampedPosition;
    }

    private void _SetCameraDampedPosition(Vector3 position)
    {
        _dampedPosition.x = position.x;
        _dampedPosition.y = position.y;
    }

    private Vector3 _ClampPositionIntoBounds(Vector3 position)
    {
        if (!_currentCameraProfile.HasBounds) return position;

        Rect BoundsRect = _currentCameraProfile.BoundsRect;
        Vector3 worldBottomLeft = _camera.ScreenToWorldPoint(new Vector3(0, 0));
        Vector3 worldTopRight= _camera.ScreenToWorldPoint(new Vector3(_camera.pixelWidth, _camera.pixelHeight));
        Vector2 worldScreenSize = new Vector2(worldTopRight.x - worldBottomLeft.x, worldTopRight.y - worldBottomLeft.y);
        Vector2 worldHalfScreenSize = worldScreenSize / 2f;

        if (position.x > BoundsRect.xMax - worldHalfScreenSize.x)
        {
            position.x = BoundsRect.xMax - worldHalfScreenSize.x;
        }
        if (position.x < BoundsRect.xMin + worldHalfScreenSize.x)
        {
            position.x = BoundsRect.xMin + worldHalfScreenSize.x;
        }

        if (position.y > BoundsRect.yMax - worldHalfScreenSize.y)
        {
            position.y = BoundsRect.yMax - worldHalfScreenSize.y;
        }
        if (position.y < BoundsRect.yMin + worldHalfScreenSize.y)
        {
            position.y = BoundsRect.yMin + worldHalfScreenSize.y;
        }

        return position;
    }
}
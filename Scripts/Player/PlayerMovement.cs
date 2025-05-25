using KINEMATION.KAnimationCore.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Input;
using UnityEngine;


public enum CharacterMovementState
{
    Idle,
    Walking,
    Sprinting,
    InAir,
    Sliding
}

public enum CharacterPoseState
{
    Standing,
    Crouching,
    Prone
}

public class PlayerMovement : MonoBehaviour
{
    public delegate bool ActionConditionDelegate();
    public delegate void OnActionCallback();

    [SerializeField] private CharacterMovementSettingsSO movementSettings;

    public OnActionCallback onStartMoving;
    public OnActionCallback onStopMoving;

    public OnActionCallback onSprintStarted;
    public OnActionCallback onSprintEnded;

    public OnActionCallback onCrouch;
    public OnActionCallback onUncrouch;

    public OnActionCallback onProneStarted;
    public OnActionCallback onProneEnded;

    public OnActionCallback onJump;
    public OnActionCallback onLanded;

    public OnActionCallback onSlideStarted;
    public OnActionCallback onSlideEnded;

    public ActionConditionDelegate _slideActionCondition;
    public ActionConditionDelegate _proneActionCondition;
    public ActionConditionDelegate _sprintActionCondition;

    private CharacterController _controller;
    private PlayerInputManager _playerInputManager;
    public CharacterMovementState MovementState { get; private set; }
    public CharacterPoseState PoseState { get; private set; }
    public Vector3 MoveVector { get; private set; }

    private Vector3 _velocity;

    private float _originalHeight;
    private Vector3 _originalCenter;

    private CharacterMovementState _cachedMovementState;

    private GaitSettings _desiredGait;
    private GaitSettings _cachedGait;
    private float _slideProgress = 0f;

    private Vector3 _prevPosition;
    private Vector3 _slideVector;

    private static readonly int InAir = Animator.StringToHash("InAir");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int Crouching = Animator.StringToHash("Crouching");
    private static readonly int Sprinting = Animator.StringToHash("Sprinting");
    private static readonly int Proning = Animator.StringToHash("Proning");

    private bool _wasMoving = false;

    private UserInputController _inputController;
    private Animator _animator;
    private bool _consumeMoveInput = true;

    private float _gaitProgress;

    private void UpdateMovementState()
    {
        if (MovementState == CharacterMovementState.Sliding && !Mathf.Approximately(_slideProgress, 1f))
        {
            // Consume input, but do not allow cancelling sliding.
            return;
        }

        if (MovementState == CharacterMovementState.InAir)
        {
            return;
        }

        // If still can sprint, keep the sprinting state.
        if (MovementState == CharacterMovementState.Sprinting
            && _playerInputManager.move.y > 0f && Mathf.Approximately(_playerInputManager.move.x, 0f))
        {
            return;
        }

        if (!IsMoving())
        {
            MovementState = CharacterMovementState.Idle;
            return;
        }

        MovementState = CharacterMovementState.Walking;
    }
    private void UpdateGrounded()
    {
        var normInput = _playerInputManager.move.normalized;
        var targetDirection = transform.right * normInput.x + transform.forward * normInput.y;

        float maxAccelTime = movementSettings.accelerationCurve.keys[^1].time;
        _gaitProgress = Mathf.Min(_gaitProgress + Time.deltaTime, maxAccelTime);

        float t = movementSettings.accelerationCurve.Evaluate(_gaitProgress);
        t = Mathf.Lerp(_cachedGait.velocity, _desiredGait.velocity, t);

        targetDirection *= Mathf.Lerp(_cachedGait.velocity, _desiredGait.velocity, t);

        targetDirection = Vector3.Lerp(_velocity, targetDirection,
            KMath.ExpDecayAlpha(_desiredGait.velocitySmoothing, Time.deltaTime));

        _velocity = targetDirection;

        targetDirection.y = -2f;
        MoveVector = targetDirection;
    }
    public bool IsMoving()
    {
        return !Mathf.Approximately(_playerInputManager.move.normalized.magnitude, 0f);
    }

    private bool CanUnCrouch()
    {
        float height = _originalHeight - _controller.radius * 2f;
        Vector3 position = transform.TransformPoint(_originalCenter + Vector3.up * height / 2f);
        return !Physics.CheckSphere(position, _controller.radius, movementSettings.groundMask);
    }

    private void Crouch()
    {
        float crouchedHeight = _originalHeight * movementSettings.crouchRatio;
        float heightDifference = _originalHeight - crouchedHeight;

        _controller.height = crouchedHeight;

        // Adjust the center position so the bottom of the capsule remains at the same position
        Vector3 crouchedCenter = _originalCenter;
        crouchedCenter.y -= heightDifference / 2;
        _controller.center = crouchedCenter;

        PoseState = CharacterPoseState.Crouching;

        _animator.SetBool(Crouching, true);
        onCrouch?.Invoke();
    }

    private void UnCrouch()
    {
        _controller.height = _originalHeight;
        _controller.center = _originalCenter;

        PoseState = CharacterPoseState.Standing;

        _animator.SetBool(Crouching, false);
        onUncrouch?.Invoke();
    }

    private void UpdateMovement()
    {
        _controller.Move(MoveVector * Time.deltaTime);
    }

    private void OnMovementStateChanged()
    {
        if (_cachedMovementState == CharacterMovementState.InAir)
        {
            onLanded?.Invoke();
        }

        if (_cachedMovementState == CharacterMovementState.Sprinting)
        {
            onSprintEnded?.Invoke();
        }

        if (_cachedMovementState == CharacterMovementState.Sliding)
        {
            onSlideEnded?.Invoke();

            if (CanUnCrouch())
            {
                UnCrouch();
            }
        }

        if (MovementState == CharacterMovementState.Idle)
        {
            _desiredGait = movementSettings.idle;
            return;
        }

        if (MovementState == CharacterMovementState.InAir)
        {
            onJump?.Invoke();
            return;
        }

        if (MovementState == CharacterMovementState.Sprinting)
        {
            _gaitProgress = 0f;
            _cachedGait = _desiredGait;

            onSprintStarted?.Invoke();
            _desiredGait = movementSettings.sprinting;
            return;
        }

        if (MovementState == CharacterMovementState.Sliding)
        {
            _desiredGait.velocitySmoothing = movementSettings.slideDirectionSmoothing;
            _slideVector = _velocity;
            _slideProgress = 0f;
            onSlideStarted?.Invoke();
            Crouch();
            return;
        }

        if (PoseState == CharacterPoseState.Crouching)
        {
            _desiredGait = movementSettings.crouching;
            return;
        }

        if (PoseState == CharacterPoseState.Prone)
        {
            _gaitProgress = 0f;
            _cachedGait = _desiredGait;
            _desiredGait = movementSettings.prone;
            return;
        }

        if (_cachedMovementState == CharacterMovementState.Idle)
        {
            _gaitProgress = 0f;
            _cachedGait = _desiredGait;
        }

        // Walking state
        _desiredGait = movementSettings.walking;
    }


    void Start()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateMovementState();
        if(_cachedMovementState != MovementState)
        {
            OnMovementStateChanged();
        }
        bool isMoving = IsMoving();

        if(_wasMoving != isMoving)
        {
            if(isMoving)
            {
                onStartMoving?.Invoke();
            }
            else
            {
                onStopMoving?.Invoke();
            }
        }

        UpdateGrounded();

        UpdateMovement();

        _cachedMovementState = MovementState;

    }
}

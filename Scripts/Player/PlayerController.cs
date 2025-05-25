using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using KINEMATION.FPSAnimationFramework.Runtime.Camera;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.FPSAnimationFramework.Runtime.Playables;
using KINEMATION.KAnimationCore.Runtime.Input;
using KINEMATION.ProceduralRecoilAnimationSystem.Runtime;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using System.Collections.Generic;
using KINEMATION.KAnimationCore.Runtime.Rig;

public enum CharacterAimState
{
    None,
    Ready,
    Aiming,
    PointAiming
}

public enum CharacterActionState
{
    None,
    PlayingAnimation,
    WeaponChange,
    AttachmentEditing
}

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    public float movementSpeed = 4.0f;
    public float sprintSpeed = 6.0f;
    public float RotationSpeed = 1.0f;
    public float speedChangeRate = 10.0f;

    [Space(10)]
    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;

    [Space(10)]
    public float jumpTimeout = 0.1f;
    public float fallTimeout = 0.15f;

    [Header("player Grounded")]
    public bool grounded = true;
    public float groundedOffset = 0.15f;
    public float groundedRadius = 0.5f;
    public LayerMask groundLayers;

    [Header("Cinemachine")]
    public GameObject cinemachineCameraTarget;
    public Transform adsTarget;
    public CinemachineVirtualCamera customCam;
    public float topClamp = 90.0f;
    public float bottomClamp = -90.0f;
    //recoil pattern
    private float basePitch = 0f;
    private float baseYaw = 0f;


    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 50.0f;

    //timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private CharacterController _controller;
    private PlayerInputManager _input;
    private GameObject _mainCamera;
    private Player _player;

    private const float _threshold = 0.01f;
    /// if Inventory open, stop rotate camera
    private PlayerInventoryController _inventoryController;

    /// <summary>
    /// FPS Animator
    /// </summary>
    [Header("Controller Settings")]
    [SerializeField] private CharacterContollerSettings settings;

    private PlayerMovement _movementComponent;

    private Transform _weaponBone;

    private int _activeWeaponIndex;
    private int _previousWeaponIndex;

    private CharacterAimState _aimState;
    private CharacterActionState _actionState;

    private FPSAnimator _fpsAnimator; //Central management component.
    private UserInputController _userInput; //Dynamic input system.
    private FPSCameraController _fpsCamera; //Camera system.
    private IPlayablesController _playablesController; //Dynamic animation system.
    private RecoilAnimation _recoilAnimation; //Recoil effect component.

    //생성된 무기
    private List<FPSItem> _instantiatedWeapons;

    //마우스 입력
    private Vector2 _lookDeltaInput;
    private Vector2 playerInput;

    private int _sensitivityMultiplierPropertyIndex;
    //반동패턴
    private RecoilPattern _recoilPattern;

    private Animator _animator;

    private static int _fullBodyWeightHash = Animator.StringToHash("FullBodyWeight");
    private static int _proneWeightHash = Animator.StringToHash("ProneWeight");
    private static int _inspectStartHash = Animator.StringToHash("InspectStart");
    private static int _inspectEndHash = Animator.StringToHash("InspectEnd");
    private static int _slideHash = Animator.StringToHash("Sliding");

    private void PlayTransitionMotion(FPSAnimatorLayerSettings layerSettings)
    {
        if (layerSettings == null)
        {
            return;
        }

        _fpsAnimator.LinkAnimatorLayer(layerSettings);
    }
    private bool IsSprinting()
    {
        return _movementComponent.MovementState == CharacterMovementState.Sprinting;
    }

    private bool HasActiveAction()
    {
        return _actionState != CharacterActionState.None;
    }
    private bool IsAiming()
    {
        return _aimState is CharacterAimState.Aiming or CharacterAimState.PointAiming;
    }

    private void InitializeMovement()
    {
        _movementComponent = GetComponent<PlayerMovement>();

        _movementComponent.onJump = () => { PlayTransitionMotion(settings.jumpingMotion); };
        _movementComponent.onLanded = () => { PlayTransitionMotion(settings.jumpingMotion); };

        _movementComponent.onCrouch = OnCrouch;
        _movementComponent.onUncrouch = OnUncrouch;

        _movementComponent.onSprintStarted = OnSprintStarted;
        _movementComponent.onSprintEnded = OnSprintEnded;

        _movementComponent.onSlideStarted = OnSlideStarted;

        _movementComponent._slideActionCondition += () => !HasActiveAction();
        _movementComponent._sprintActionCondition += () => !HasActiveAction();
        _movementComponent._proneActionCondition += () => !HasActiveAction();

        _movementComponent.onStopMoving = () =>
        {
            PlayTransitionMotion(settings.stopMotion);
        };

        _movementComponent.onProneEnded = () =>
        {
            _userInput.SetValue(FPSANames.PlayablesWeight, 1f);
        };
    }

    private void InitializeWeapons()
    {
        _instantiatedWeapons = new List<FPSItem>();

        foreach (var prefab in settings.weaponPrefabs)
        {
            var weapon = Instantiate(prefab, transform.position, Quaternion.identity);

            var weaponTransform = weapon.transform;

            weaponTransform.parent = _weaponBone;
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;

            _instantiatedWeapons.Add(weapon.GetComponent<FPSItem>());
            weapon.gameObject.SetActive(false);
        }
    }

    private void OnSlideStarted()
    {
        _animator.CrossFade(_slideHash, 0.2f);
    }

    private void OnSprintStarted()
    {
        OnFireReleased();
        DisableAim();

        _aimState = CharacterAimState.None;

        _userInput.SetValue(FPSANames.StabilizationWeight, 0f);
        _userInput.SetValue("LookLayerWeight", 0.3f);
    }

    private void OnSprintEnded()
    {
        _userInput.SetValue(FPSANames.StabilizationWeight, 1f);
        _userInput.SetValue("LookLayerWeight", 1f);
    }

    private void OnCrouch()
    {
        PlayTransitionMotion(settings.crouchingMotion);
    }

    private void OnUncrouch()
    {
        PlayTransitionMotion(settings.crouchingMotion);
    }


    private void DisableAim()
    {
        if (GetActiveItem().OnAimReleased()) _aimState = CharacterAimState.None;
    }

    private void OnFirePressed()
    {
        if (_instantiatedWeapons.Count == 0 || HasActiveAction()) return;
        GetActiveItem().OnFirePressed();
    }

    private void OnFireReleased()
    {
        if (_instantiatedWeapons.Count == 0) return;
        GetActiveItem().OnFireReleased();
    }

    private FPSItem GetActiveItem()
    {
        if (_instantiatedWeapons.Count == 0) return null;
        return _instantiatedWeapons[_activeWeaponIndex];
    }

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _fpsAnimator = GetComponent<FPSAnimator>();
        _fpsAnimator.Initialize();

        _userInput = GetComponent<UserInputController>();
        _fpsCamera = GetComponentInChildren<FPSCameraController>();
        _playablesController = GetComponent<IPlayablesController>();
        _recoilAnimation = GetComponent<RecoilAnimation>();
        _recoilPattern = GetComponent<RecoilPattern>();
        _animator = GetComponent<Animator>();

        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputManager>();
        _inventoryController = GetComponent<PlayerInventoryController>();
        _player = GetComponent<Player>();
        _playerInput = GetComponent<PlayerInput>();

        _weaponBone = GetComponentInChildren<KRigComponent>().GetRigTransform(settings.weaponBone);
        


        InitializeMovement();
        InitializeWeapons();

        _actionState = CharacterActionState.None;
        EquipWeapon();

        _sensitivityMultiplierPropertyIndex = _userInput.GetPropertyIndex("SensitivityMultiplier");

    }

    private void Update()
    {
        UpdateLookInput();
        OnMovementUpdated();
    }

    private void UnequipWeapon()
    {
        DisableAim();
        _actionState = CharacterActionState.WeaponChange;
        GetActiveItem().OnUnEquip();
    }

    public void ResetActionState()
    {
        _actionState = CharacterActionState.None;
    }

    private void EquipWeapon()
    {
        if (_instantiatedWeapons.Count == 0) return;

        _instantiatedWeapons[_previousWeaponIndex].gameObject.SetActive(false);
        GetActiveItem().gameObject.SetActive(true);
        GetActiveItem().OnEquip(gameObject);

        _actionState = CharacterActionState.None;
    }

    private void StartWeaponChange(int newIndex)
    {
        if (newIndex == _activeWeaponIndex || newIndex > _instantiatedWeapons.Count - 1)
        {
            return;
        }

        UnequipWeapon();

        OnFireReleased();
        Invoke(nameof(EquipWeapon), settings.equipDelay);

        _previousWeaponIndex = _activeWeaponIndex;
        _activeWeaponIndex = newIndex;
    }

    private void UpdateLookInput()
    {
        float scale = _userInput.GetValue<float>(_sensitivityMultiplierPropertyIndex);
        float deltaMouseX = _input.look.x * RotationSpeed * scale;
        float deltaMouseY = -_input.look.y * RotationSpeed * scale;

        playerInput.y += deltaMouseY;
        playerInput.x += deltaMouseX;

        if (_recoilPattern != null)
        {
            playerInput += _recoilPattern.GetRecoilDelta();
            deltaMouseX += _recoilPattern.GetRecoilDelta().x;
        }

        float proneWeight = _animator.GetFloat(_proneWeightHash);
        Vector2 pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);

        playerInput.y = Mathf.Clamp(playerInput.y, pitchClamp.x, pitchClamp.y);

        transform.rotation *= Quaternion.Euler(0f, deltaMouseX, 0f);

        _userInput.SetValue(FPSANames.MouseDeltaInput, new Vector4(deltaMouseX, deltaMouseY));
        _userInput.SetValue(FPSANames.MouseInput, new Vector4(playerInput.x, playerInput.y));
    }

    private void OnMovementUpdated()
    {
        float playablesWeight = 1f - _animator.GetFloat(_fullBodyWeightHash);
        _userInput.SetValue(FPSANames.PlayablesWeight, playablesWeight);
    }


    private Vector2 currentRecoil;
    private Vector2 targetRecoil;
    public void ApplyCameraRecoil(Vector2 recoil)
    {
        targetRecoil += recoil;
    }

}

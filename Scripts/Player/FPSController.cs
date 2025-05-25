using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using KINEMATION.KAnimationCore.Runtime.Input;
using KINEMATION.KAnimationCore.Runtime.Rig;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


public enum FPSAimState
{
    None,
    Ready,
    Aiming,
    PointAiming
}

public enum FPSActionState
{
    None,
    PlayingAnimation,
    WeaponChange,
    AttachmentEditing,
    UsingItem
}

[RequireComponent(typeof(CharacterController), typeof(FPSMovement))]
public class FPSController : MonoBehaviour
{
    [SerializeField] private FPSControllerSettings settings;

    private FPSMovement _movementComponent;
    private PlayerInputManager _input;
    private Player _player;

    private Transform _weaponBone;
    private Vector2 _playerInput;

    private int _activeWeaponIndex;
    private int _previousWeaponIndex;

    private int _firstWeaponIndex = 1;
    private int _secondWeaponIndex = 2;

    //연발 관리 프로퍼티
    private bool _isFiring = false;
    private Coroutine _autoFireCoroutine;
    private float _fireInterval;
    //단발 시간 확인 interval
    [SerializeField] private float _interval = 0;

    private FPSAimState _aimState;
    public FPSActionState _actionState;

    private Animator _animator;

    // ~Scriptable Animation System Integration
    private FPSAnimator _fpsAnimator;
    private UserInputController _userInput;
    // ~Scriptable Animation System Integration

    [SerializeField] public List<FPSItem> _instantiatedWeapons { get; private set; }
    public int instantiateWeaponsCount { get; private set; }

    private RecoilPattern _recoilPattern;
    private int _sensitivityMultiplierPropertyIndex;

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
        return _movementComponent.MovementState == FPSMovementState.Sprinting;
    }

    private bool IsUsingItem()
    {
        return _actionState == FPSActionState.UsingItem;
    }

    private bool HasActiveAction()
    {
        return _actionState != FPSActionState.None;
    }

    private bool IsAiming()
    {
        return _aimState is FPSAimState.Aiming or FPSAimState.PointAiming;
    }

    public int GetActiveWeaponIndex()
    {
        return _activeWeaponIndex;
    }

    private void InitializeMovement()
    {
        _movementComponent = GetComponent<FPSMovement>();

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
        //_instantiatedWeapons = new List<FPSItem>();

        foreach (var prefab in settings.weaponPrefabs)
        {
            if (prefab == null)
            {
                _instantiatedWeapons.Add(null);
                continue;
            }
            var weapon = Instantiate(prefab, transform.position, Quaternion.identity);

            var weaponTransform = weapon.transform;

            weaponTransform.parent = _weaponBone;
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;

            _instantiatedWeapons.Add(weapon.GetComponent<FPSItem>());
            weapon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 무기를 Player Body에 생성하는 메서드 무기를 Change하거나 새로 Init할때도 항상 해당 메서드 사용
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public GameObject InitWeapon(GameObject prefab, int slotIndex)
    {
        //Init할 weapon Slot Index를 저장할 변수
        slotIndex++;
        int storeWeaponIndex;
        //무기를 바꾸었는지 체크하는 bool 변수
        bool isWeaponChanged = false;
        //슬롯에 Weapon이 있는 경우
        if (_instantiatedWeapons[slotIndex] != null)
        {
            UnequipWeapon();
            OnFireReleased();
            _instantiatedWeapons[slotIndex] = null;
            isWeaponChanged = true;
        }

        storeWeaponIndex = slotIndex;
        /*if (_instantiatedWeapons[_firstWeaponIndex] == null) storeWeaponIndex = _firstWeaponIndex;
        else if (_instantiatedWeapons[_secondWeaponIndex] == null) storeWeaponIndex = _secondWeaponIndex;*/

        var weapon = Instantiate(prefab, transform.position, Quaternion.identity);

        var weaponTransform = weapon.transform;

        weaponTransform.parent = _weaponBone;
        weaponTransform.localPosition = Vector3.zero;
        weaponTransform.localRotation = Quaternion.identity;

        _instantiatedWeapons[storeWeaponIndex] = weapon.GetComponent<FPSItem>();

        //무기가 바뀐경우 바로 Active하고 Equip해준다.
        if(isWeaponChanged)
        {
            _activeWeaponIndex = storeWeaponIndex;
            EquipWeapon();
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }
        return weapon;
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 144;

        _fpsAnimator = GetComponent<FPSAnimator>();
        _fpsAnimator.Initialize();

        _weaponBone = GetComponentInChildren<KRigComponent>().GetRigTransform(settings.weaponBone);
        _animator = GetComponent<Animator>();

        _userInput = GetComponent<UserInputController>();
        _recoilPattern = GetComponent<RecoilPattern>();

        _input = GetComponent<PlayerInputManager>();
        _player = GetComponent<Player>();

        _instantiatedWeapons = new List<FPSItem>();
        InitializeMovement();
        InitializeWeapons();

        _actionState = FPSActionState.None;
        EquipWeapon();

        _sensitivityMultiplierPropertyIndex = _userInput.GetPropertyIndex("SensitivityMultiplier");
    }

    private void UnequipWeapon()
    {
        DisableAim();
        _actionState = FPSActionState.WeaponChange;
        GetActiveItem().OnUnEquip();
        _player.gunController.RemoveWeapon();
        _player.gunController.RemoveGreande();
    }

    public void ResetActionState()
    {
        _actionState = FPSActionState.None;
    }

    private void EquipWeapon()
    {
        if (_instantiatedWeapons.Count == 0 || IsUsingItem()) return;

        if (_instantiatedWeapons[_previousWeaponIndex] != null)
            _instantiatedWeapons[_previousWeaponIndex].gameObject.SetActive(false);

        GetActiveItem().gameObject.SetActive(true);
        GetActiveItem().OnEquip(gameObject);

        if (GetActiveItem().TryGetComponent<Gun>(out Gun component))
        {
            _player.gunController.currentGun = component;
        }
        if (GetActiveItem().TryGetComponent<Grenade>(out Grenade cmp))
        {
            _player.gunController.currentGrenade = cmp;
        }

        _actionState = FPSActionState.None;
    }

    private void DisableAim()
    {
        if (GetActiveItem() == null) return;
        if (GetActiveItem().OnAimReleased()) _aimState = FPSAimState.None;
    }

    private void OnFirePressed()
    {
        if (_instantiatedWeapons.Count == 0 || HasActiveAction()) return;
        GetActiveItem().OnFirePressed();
    }

    private void OnFireReleased()
    {
        if (_instantiatedWeapons.Count == 0) return;
        if (GetActiveItem() == null) return;
        GetActiveItem().OnFireReleased();
    }

    private FPSItem GetActiveItem()
    {
        if (_instantiatedWeapons.Count == 0) return null;
        return _instantiatedWeapons[_activeWeaponIndex];
    }

    private void OnSlideStarted()
    {
        _animator.CrossFade(_slideHash, 0.2f);
    }

    private void OnSprintStarted()
    {
        OnFireReleased();
        DisableAim();

        _aimState = FPSAimState.None;

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

    private bool _isLeaning;

    private bool StartWeaponChange(int newIndex)
    {
        if (newIndex == _activeWeaponIndex || newIndex > _instantiatedWeapons.Count - 1 || _instantiatedWeapons[newIndex] == null)
        {
            return false;
        }

        UnequipWeapon();

        OnFireReleased();
        Invoke(nameof(EquipWeapon), settings.equipDelay);

        _previousWeaponIndex = _activeWeaponIndex;
        _activeWeaponIndex = newIndex;
        return true;
    }


    private void UpdateLookInput()
    {
        float scale = _userInput.GetValue<float>(_sensitivityMultiplierPropertyIndex);

        float deltaMouseX = _input.look.x * settings.sensitivity * scale;
        float deltaMouseY = -_input.look.y * settings.sensitivity * scale;

        _playerInput.y += deltaMouseY;
        _playerInput.x += deltaMouseX;

        if (_recoilPattern != null)
        {
            _playerInput += _recoilPattern.GetRecoilDelta();
            deltaMouseX += _recoilPattern.GetRecoilDelta().x;
        }

        float proneWeight = _animator.GetFloat(_proneWeightHash);
        Vector2 pitchClamp = Vector2.Lerp(new Vector2(-90f, 90f), new Vector2(-30, 0f), proneWeight);

        _playerInput.y = Mathf.Clamp(_playerInput.y, pitchClamp.x, pitchClamp.y);

        transform.rotation *= Quaternion.Euler(0f, deltaMouseX, 0f);

        _userInput.SetValue(FPSANames.MouseDeltaInput, new Vector4(deltaMouseX, deltaMouseY));
        _userInput.SetValue(FPSANames.MouseInput, new Vector4(_playerInput.x, _playerInput.y));
    }

    private void OnMovementUpdated()
    {
        float playablesWeight = 1f - _animator.GetFloat(_fullBodyWeightHash);
        _userInput.SetValue(FPSANames.PlayablesWeight, playablesWeight);
    }

    private void Update()
    {
        Time.timeScale = settings.timeScale;
        UpdateLookInput();
        OnMovementUpdated();
        _interval += Time.deltaTime;
    }

#if ENABLE_INPUT_SYSTEM
    public void OnReload(InputAction.CallbackContext context)
    {
        if (IsSprinting() || HasActiveAction()) return;
        if (context.performed)
        {
            if (!_player.gunController.ReloadWeapon()) return;
            if (!GetActiveItem().OnReload()) return;
            if (_isFiring)
            {
                if (_autoFireCoroutine != null)
                {
                    StopCoroutine(AutoFireRoutine());
                }
                _isFiring = false;
                OnFireReleased();
            }
            _actionState = FPSActionState.PlayingAnimation;
        }
    }

    public void OnThrowGrenade()
    {
        if (IsSprinting() || HasActiveAction() || !GetActiveItem().OnGrenadeThrow()) return;
        _actionState = FPSActionState.PlayingAnimation;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (IsSprinting() || !_player.playerInputManager.cursorInputForLook || IsUsingItem()) return;
        if (_actionState == FPSActionState.PlayingAnimation) return;

        var weapon = GetActiveItem();
        if (weapon == null) return;

        //밀리 or 수류탄
        if (weapon.OnGetWeaponType() == Define.WeaponType.melee || weapon.OnGetWeaponType() == Define.WeaponType.grenade)
        {
            if (context.performed)
            {
                OnFirePressed();
            }
            else if (context.canceled)
            {
                OnFireReleased();
            }
            return;
        }

        //연발
        if (_player.gunController.currentGun != null &&
            _player.gunController.currentGun.gunData.isAutomatic)
        {
            if (context.performed)
            {
                _isFiring = true;
                _autoFireCoroutine = StartCoroutine(AutoFireRoutine());
            }
            else if (context.canceled)
            {
                _isFiring = false;
                if (_autoFireCoroutine != null)
                    StopCoroutine(_autoFireCoroutine);
                OnFireReleased();
            }
        }
        else
        {
            //단발 무기 처리
            if (context.performed)
            {
                if (_player.gunController.currentGun.gunData.shootingInterval < _interval)
                {
                    //발사가되면 interval 처리
                    if (_player.gunController.currentGun.TryFire())
                    {
                        _isFiring = true;
                        OnFirePressed();
                        _interval = 0f;
                    }
                }
            }
            if (context.canceled)
            {
                OnFireReleased();
                _isFiring = false;
            }
        }
    }


    private IEnumerator AutoFireRoutine()
    {
        _fireInterval = _player.gunController.currentGun.gunData.shootingInterval;
        while (_isFiring && _player.gunController.currentGun != null)
        {
            if (_player.gunController.currentGun.TryFire())
            {
                OnFirePressed();
            }
            yield return new WaitForSeconds(_fireInterval);
        }
        yield return null;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (IsSprinting() || IsUsingItem() || GetActiveItem() == null) return;
        if (context.performed && !IsAiming())
        {
            if (GetActiveItem().OnAimPressed()) _aimState = FPSAimState.Aiming;
            PlayTransitionMotion(settings.aimingMotion);
            _player.playerUI.inGameUI.ui_crossHair.DeActiveCrossHairWeapon();
            return;
        }

        if (!context.performed && IsAiming())
        {
            DisableAim();
            PlayTransitionMotion(settings.aimingMotion);
            _player.playerUI.inGameUI.ui_crossHair.ActiveCrossHairWeapon();
        }
    }

    public void OnChangeWeapon(InputAction.CallbackContext context)
    {
        if (_movementComponent.PoseState == FPSPoseState.Prone) return;
        if (HasActiveAction() || _instantiatedWeapons.Count == 0) return;
        string weaponId = context.control.displayName;
        if (StartWeaponChange(int.Parse(weaponId) - 1))
        {
            if (int.Parse(weaponId) == 1) return;
            _player.playerUI.inventoryUI.weaponSlots[int.Parse(weaponId) - 2].SetInGameWeaponUI();

        }
        //StartWeaponChange(_activeWeaponIndex + 1 > _instantiatedWeapons.Count - 1 ? 0 : _activeWeaponIndex + 1);
    }

    public void ChangeGrenade(int greandeId)
    {
        if (_movementComponent.PoseState == FPSPoseState.Prone) return;
        if (HasActiveAction() || _instantiatedWeapons.Count == 0) return;
        StartWeaponChange(greandeId - 1);
    }


    public void OnLean(InputValue value)
    {
        _userInput.SetValue(FPSANames.LeanInput, value.Get<float>() * settings.leanAngle);
        PlayTransitionMotion(settings.leanMotion);
    }

    public void OnCycleScope()
    {
        if (!IsAiming()) return;

        GetActiveItem().OnCycleScope();
        PlayTransitionMotion(settings.aimingMotion);
    }

    public void OnChangeFireMode()
    {
        GetActiveItem().OnChangeFireMode();
    }

    public void OnToggleAttachmentEditing()
    {
        if (HasActiveAction() && _actionState != FPSActionState.AttachmentEditing) return;

        _actionState = _actionState == FPSActionState.AttachmentEditing
            ? FPSActionState.None : FPSActionState.AttachmentEditing;

        if (_actionState == FPSActionState.AttachmentEditing)
        {
            _animator.CrossFade(_inspectStartHash, 0.2f);
            return;
        }

        _animator.CrossFade(_inspectEndHash, 0.3f);
    }

    public void OnDigitAxis(InputValue value)
    {
        if (!value.isPressed || _actionState != FPSActionState.AttachmentEditing) return;
        GetActiveItem().OnAttachmentChanged((int)value.Get<float>());
    }
#endif
}

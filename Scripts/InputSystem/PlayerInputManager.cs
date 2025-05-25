using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Player Input Value")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool fire;
    public bool isInventoryToggle;
    public bool interact;
    public bool zoomIn;

    [Header("Movement Settings")]
    public bool analogMovement;
    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private PlayerInventoryController _inventoryController;
    private PlayerEquipManager _equipManager;
    private Skill _skill;
    private Player _player;
    private void Start()
    {
        _inventoryController = GetComponent<PlayerInventoryController>();
        _equipManager = GetComponent<PlayerEquipManager>();
        _player = GetComponent<Player>();
        _skill = GetComponent<Skill>();
    }
    public void IsLookLock(bool isLock)
    {
        if(isLock)
        {
            cursorInputForLook = false;
            look = Vector2.zero;
        }
        else
        {
            cursorInputForLook = true;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (cursorInputForLook)
        {
            LookInput(context.ReadValue<Vector2>());
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpInput(context.performed);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintInput(context.performed);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        FireInput(context.performed);
    }

    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        ToggleInventoryInput(context.performed);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        InteractInput(context.performed);
    }

    public void OnToggleWeapon(InputAction.CallbackContext context)
    {
        ToggleWeaponInput(context.control.displayName);
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        ReloadInput(context.performed);
    }

    public void OnZoomIn(InputAction.CallbackContext context)
    {
        ZoomInInput(context.performed);
    }
    private bool isHolding = false;
    private float holdThreshold = 0.5f;
    private bool actionTriggered = false;
    public void OnToggleGrenade(InputAction.CallbackContext context)
    {
        if (context.started) //버튼이 눌리기 시작할 때
        {
            isHolding = true;
            actionTriggered = false;
            Invoke(nameof(HoldGrenadeInput), holdThreshold);
        }
        else if (context.canceled) //버튼을 뗄 때
        {
            if (!actionTriggered) //0.5초 안에 버튼을 떼면 클릭 동작 실행
            {
                ToggleGrenadeInput(context.performed);
            }
            isHolding = false;
            CancelInvoke(nameof(HoldGrenadeInput));
            CancelGrenadeInput();
        }
    }

    public void OnToggleHeal(InputAction.CallbackContext context)
    {
        if (context.started) //버튼이 눌리기 시작할 때
        {
            isHolding = true;
            actionTriggered = false;
            Invoke(nameof(HoldHealInput), holdThreshold);
        }
        else if (context.canceled) //버튼을 뗄 때
        {
            if (!actionTriggered) //0.5초 안에 버튼을 떼면 클릭 동작 실행
            {
                ToggleHealInput(context.performed);
            }
            isHolding = false;
            CancelInvoke(nameof(HoldHealInput));
            CancelHealInput();
        }
    }


    public void OnActiveTacticalSkill(InputAction.CallbackContext context)
    {
        ActiveTacticalSillInput();
    }
    public void OnActiveUltimateSkill(InputAction.CallbackContext context)
    {
        ActiveUltimateSillInput();
    }


    public void MoveInput(Vector2 moveDirection)
    {
        move = moveDirection;
    }

    public void LookInput(Vector2 lookDirection)
    {
        look = lookDirection;
    }

    public void JumpInput(bool jumpState)
    {
        jump = jumpState;
    }

    public void SprintInput(bool sprintState)
    {
        sprint = sprintState;
    }

    public void FireInput(bool fireState)
    {
        fire = fireState;
    }

    public void ToggleInventoryInput(bool toggleState)
    {
        _inventoryController.ToggleInventory();
    }

    public void ToggleWeaponInput(string weaponId)
    {
        _equipManager.ToggleWeapon(int.Parse(weaponId) - 1);
    }

    public void ToggleGrenadeInput(bool toggelGrenade)
    {
        _player.playerUI.grenadeSlotUI.EquipSelcetedGrenade();
    }

    public void ToggleHealInput(bool toggleHeal)
    {
        _player.playerUI.healSlotUI.UseSelectedHealItem();
    }

    public void HoldGrenadeInput()
    {
        if(isHolding)
        {
            actionTriggered = true;
            _player.playerUI.grenadeSlotUI.ToggleUI(true);
        }
    }

    public void HoldHealInput()
    {
        if (isHolding)
        {
            actionTriggered = true;
            //힐 슬롯 열기
            _player.playerUI.healSlotUI.ToggleUI(true);
        }
    }

    public void CancelHealInput()
    {
        _player.playerUI.healSlotUI.ToggleUI(false);
    }

    public void CancelGrenadeInput()
    {
        _player.playerUI.grenadeSlotUI.ToggleUI(false);
    }

    public void InteractInput(bool interactState)
    {
        interact = interactState;
    }

    public void ReloadInput(bool ReloadState)
    {
        _player.gunController.ReloadWeapon();
    }

    public void ZoomInInput(bool zoomInState)
    {
        zoomIn = zoomInState;
    }

    private void ActiveTacticalSillInput()
    {
        _skill.ActiveTacticalSkill();
    }

    private void ActiveUltimateSillInput()
    {
        _skill.ActiveUltimateSkill();
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
    }

}

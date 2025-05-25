using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [Header("InventoryUI")]
    [SerializeField] private GameObject _inventory;
    public UI_Popup_Inventory inventory;
    public Player _player;
    public bool isOpen = false;

    public void Init()
    {
        _player = GetComponent<Player>();
        inventory = _player.playerUI.inventoryUI;
        inventory.Init(_player);
    }
    public void ToggleInventory()
    {
        if (isOpen)
        {
            _inventory.SetActive(false);
            isOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            _player.playerInputManager.IsLookLock(false);
        }
        else
        {
            _inventory.SetActive(true);
            isOpen = true;
            Cursor.lockState = CursorLockMode.None;
            _player.playerInputManager.IsLookLock(true);
        }
        return;
    }
}

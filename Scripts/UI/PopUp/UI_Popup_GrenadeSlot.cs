using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Popup_GrenadeSlot : UI_Popup
{
    private Player _player;
    //각 슬롯
    public UI_SubItem_GrenadeSlot[] slots;
    public TextMeshProUGUI titlePrompt;
    public TextMeshProUGUI descPrompt;

    public int selectedIndex = -1;
    private bool isSelecting = true;

    public void Init(Player player)
    {
        _player = player;
        selectedIndex = -1;
    }
    protected override void Update()
    {
        UpdateSelection();
    }

    public void UpdateUI()
    {
        SyncGrenadesFromInventory();
    }

    private void SyncGrenadesFromInventory()
    {
        if (_player == null || _player.playerInventoryController == null) return;

        var inventorySlots = _player.playerInventoryController.inventory.slots;

        //각 힐타입별 갯수 저장용
        Dictionary<Define.GrenadeType, int> grenadeCounts = new Dictionary<Define.GrenadeType, int>();

        foreach (var slot in inventorySlots)
        {
            if (slot.item == null)
            {
                continue;
            }
            if (slot.item.itemData.type != Define.ItemType.grenade)
            {
                continue;
            }

            Define.GrenadeType grenadeType = slot.item.itemData.grenadeType;
            if (!grenadeCounts.ContainsKey(grenadeType))
            {
                grenadeCounts[grenadeType] = 0;
            }
            grenadeCounts[grenadeType] += slot.item.quantity;
        }

        //슬롯 힐 타입에 맞춰서 UI업데이트
        for (int i = 0; i < slots.Length; i++)
        {
            Define.GrenadeType slotType = slots[i].grenadeType;
            int cnt = grenadeCounts.ContainsKey(slotType) ? grenadeCounts[slotType] : 0;
            slots[i].UpdateUI(cnt);
        }
    }

    public void UpdateSelection()
    {
        Vector2 dir = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        if (angle >= 315 || angle < 45)
            selectedIndex = 0; //오른쪽
        else if (angle >= 45 && angle < 135)
            selectedIndex = 1; //위
        else if (angle >= 135 && angle < 225)
            selectedIndex = 2; //왼쪽


        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].transform.localScale = (i == selectedIndex) ? Vector3.one * 1.2f : Vector3.one;
        }

        //중앙에 정보 표시
        titlePrompt.text = slots[selectedIndex].itemNameString;
        descPrompt.text = slots[selectedIndex].descString;

        _player.playerUI.inGameUI.ui_grenade.UpdateUI(selectedIndex);
    }

    /// <summary>
    /// 마지막으로 선택된 수류탄을 인벤토리에서 꺼내서 장착
    /// </summary>
    public void EquipSelcetedGrenade()
    {
        int totalGrenadeCnt = CheckGrenadeFromInventory(selectedIndex + 4);
        if(totalGrenadeCnt > 0)
        {
            _player.fPSController.ChangeGrenade(selectedIndex + 4);
        }
    }

    public int CheckGrenadeFromInventory(int idx)
    {
        int total = 0;
        switch (idx)
        {
            case 5:
                total = _player.playerInventoryController.inventory.GetTotalGrenadeCount(Define.GrenadeType.fragGrenade); break;
            case 6:
                total = _player.playerInventoryController.inventory.GetTotalGrenadeCount(Define.GrenadeType.arcStar); break;
            case 7:
                total = _player.playerInventoryController.inventory.GetTotalGrenadeCount(Define.GrenadeType.thermite); break;
        }
        return total;
    }

    public void ToggleUI(bool isActive)
    {
        if (isActive)
        {
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            _player.playerInputManager.IsLookLock(true);
        }
        else
        {
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            _player.playerInputManager.IsLookLock(false);
        }
        UpdateUI();
    }
}



using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Popup_HealSlot : UI_Popup
{
    private Player _player;
    //�� ����
    public UI_SubItem_HealSlot[] slots;
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
    /// <summary>
    /// Inventory�� ����ȭ��
    /// </summary>
    public void UpdateUI()
    {
        SyncHealItemsFromInventory();
    }

    public void SyncHealItemsFromInventory()
    {
        if (_player == null || _player.playerInventoryController == null) return;

        var inventorySlots = _player.playerInventoryController.inventory.slots;

        //�� ��Ÿ�Ժ� ���� �����
        Dictionary<Define.HealType, int> healCounts = new Dictionary<Define.HealType, int>();

        foreach (var slot in inventorySlots)
        {
            if (slot.item == null)
            {
                continue;
            }
            if (slot.item.itemData.type != Define.ItemType.heal)
            {
                continue;
            }

            Define.HealType healType = slot.item.itemData.healType;
            if (!healCounts.ContainsKey(healType))
            {
                healCounts[healType] = 0;
            }
            healCounts[healType] += slot.item.quantity;

        }

        //���� �� Ÿ�Կ� ���缭 UI������Ʈ
        for (int i = 0; i < slots.Length; i++)
        {
            Define.HealType slotType = slots[i].healType;
            int cnt = healCounts.ContainsKey(slotType) ? healCounts[slotType] : 0;
            slots[i].UpdateUI(cnt);
        }
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

    public void UpdateSelection()
    {
        Vector2 dir = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        if (angle >= 315 || angle < 45)
            selectedIndex = 0; //������
        else if (angle >= 45 && angle < 135)
            selectedIndex = 1; //��
        else if (angle >= 135 && angle < 225)
            selectedIndex = 2; //����
        else if (angle >= 225 && angle < 315)
            selectedIndex = 3; //�Ʒ�

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].transform.localScale = (i == selectedIndex) ? Vector3.one * 1.2f : Vector3.one;
        }

        //�߾ӿ� ���� ǥ��
        titlePrompt.text = slots[selectedIndex].itemNameString;
        descPrompt.text = slots[selectedIndex].descString;

        _player.playerUI.inGameUI.ui_heal.UpdateUI(selectedIndex);
    }

    /// <summary>
    /// Ű���� 4�� Ȧ�� �����ʰ� ������ �� ���õ� �������� ����ϰ��ϴ� �ڵ�
    /// </summary>
    public void UseSelectedHealItem()
    {
        if (selectedIndex == -1) return;
        var inventorySlots = _player.playerInventoryController.inventory.slots;
        Define.HealType selectedHealType = slots[selectedIndex].healType;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            if (slot.item == null) continue;
            if (slot.item.itemData.type != Define.ItemType.heal) continue;
            if (slot.item.itemData.healType != selectedHealType) continue;

            bool isUsed = slot.item.UseItem(_player);
            if (!isUsed) return;

            slot.item.quantity -= 1;

            _player.playerInventoryController.inventory.RefilItem(slot.item);
            slot.UpdateUI();

            UpdateUI();

            break;
        }
    }


}



using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UI_Popup_Inventory : UI_Popup
{
    [Header("Slot's Parent")]
    [SerializeField]
    private GameObject slotsParent;
    [SerializeField]
    private GameObject weaponSlotsParent;
    [Header("Slot List")]
    [SerializeField]
    public UI_SubItem_ItemSlot[] slots;
    [SerializeField]
    public UI_SubItem_WeaponSlot[] weaponSlots;
    [Header("Discard Zone")]
    [SerializeField]
    public UI_SubItem_DiscardZone[] discardZone;

    private Player _player;
    //PlayerEquipManager.cs AttachWeaponRoot�� ����
    public UnityEvent<int> OnAcquireWeapon;
    public UnityEvent OnAcquireItem;


    public void Init(Player player)
    {
        slots = slotsParent.GetComponentsInChildren<UI_SubItem_ItemSlot>();
        _player = player;
        SetSlotsParents();
        weaponSlots = weaponSlotsParent.GetComponentsInChildren<UI_SubItem_WeaponSlot>();
        //�κ��丮�� Init�ɶ� �̺�Ʈ ����
        OnAcquireWeapon.AddListener(_player.playerEquipManager.AttachWeaponOnRoot);
        //weaponSlots ������Ű�� 
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].OnDiscardWeapon.AddListener(_player.playerEquipManager.DestroyWeapon);
            weaponSlots[i].Init(player);
        }
        for (int i = 0; i < discardZone.Length; i++)
        {
            discardZone[i].Init(this);
        }
    }
    /// <summary>
    /// ���Ե鿡�� �θ�� �������ֱ�
    /// </summary>
    public void SetSlotsParents()
    {
        foreach (var slot in slots)
        {
            slot.Init(this, _player);
        }
    }
    #region AcquireSystem
    /// <summary>
    /// �������� ȹ���ϴ� �ڵ�
    /// </summary>
    /// <param name="item"></param>
    public void AcquireItem(Item item)
    {
        if (slots == null)
        {
            Debug.Log("Slots NULL!!");
            return;
        }

        int remainingQuantity = item.quantity; //���� ������ ����

        //������ �� ���� ��ȸ�ϸ鼭 ���� ���� Ȯ�� �� �� ���� ã��
        UI_SubItem_ItemSlot emptySlot = null;

        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item.itemData.type == item.itemData.type)
            {
                if (item.itemData.type == Define.ItemType.bullet && slot.item.itemData.bulletType != item.itemData.bulletType) continue;
                if (item.itemData.type == Define.ItemType.heal && slot.item.itemData.healType != item.itemData.healType) continue;
                int canAdd = Mathf.Min(remainingQuantity, slot.item.itemData.maxCount - slot.item.quantity);
                slot.item.quantity += canAdd;
                slot.UpdateUI();
                remainingQuantity -= canAdd;

                if (remainingQuantity <= 0)
                {
                    SortInventory();
                    OnAcquireItem?.Invoke();
                    return;
                }
            }
            else if (slot.item == null && emptySlot == null)
            {
                emptySlot = slot; //�� ���� ã��
            }
        }
        if (emptySlot == null)
        {
            Debug.Log("Inventory is Full");
            return;
        }

        //���� �������� �ִٸ� �� ���Կ� �߰�
        if (remainingQuantity > 0 && emptySlot != null)
        {
            emptySlot.AddItem(new Item(item.itemData, remainingQuantity));
            emptySlot.SetImage();
            SortInventory();
            OnAcquireItem?.Invoke();
        }
    }

    public void AcquireWeapon(Item item)
    {
        if (weaponSlots != null)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                //Debug.Log(slots[i]);
                if (weaponSlots[i].isActive == false && weaponSlots[i].item == null)
                {
                    SetWeapon(item, i);
                    return;
                }
            }
            //��� ������ �������� �� ���ִ� ���
            Debug.Log("������ �ִ� ���⸦ ������ ��ü�մϴ�");
            int curWeaponId = _player.fPSController.GetActiveWeaponIndex();
            //���� weaponSlot�� 0,1�� ������ GetActiveWeaopnIndex�޼���� 1,2������ ������
            ChangeWeapon(item, curWeaponId - 1);
        }
        else
        {
            Debug.Log("Slots NULL!!");
        }
    }

    /// <summary>
    /// ���� ��ü �ý���, ���� ������ + ���� ���� �޼��带 �Ѵ� �������ִ�.
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="curWeaponId"></param>
    public void ChangeWeapon(Item newItem, int curWeaponId)
    {
        DiscardWeapon(curWeaponId);
        SetWeapon(newItem, curWeaponId);
    }
    /// <summary>
    /// ����ź ������ ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="grenadeType"></param>
    /// <returns></returns>
    public int GetTotalGrenadeCount(Define.GrenadeType grenadeType)
    {
        int total = 0;

        foreach (var slot in slots)
        {
            if (slot.item != null &&
                slot.item.itemData.grenadeType == grenadeType)
            {
                total += slot.item.quantity;
            }
        }

        return total;
    }


    /// <summary>
    /// ������ ����ź�� ������
    /// ����ϴ� �Լ�
    /// </summary>
    /// <param name="grenadeType"></param>
    public void ConsumeGrenade(Define.GrenadeType grenadeType)
    {
        foreach (var slot in slots)
        {
            if (slot.item != null &&
                slot.item.itemData.grenadeType == grenadeType)
            {
                slot.item.quantity--;
                slot.UpdateUI();

                SortInventory(); //����
                return;
            }
        }
    }

    /// <summary>
    /// ���⸦ ������ �� ����ϴ� �޼���
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="curWeaponId"></param>
    public void SetWeapon(Item newItem, int curWeaponId)
    {
        weaponSlots[curWeaponId].AddItem(newItem);
        weaponSlots[curWeaponId].SetImage();
        weaponSlots[curWeaponId].isActive = true;
        OnAcquireWeapon?.Invoke(curWeaponId);
    }

    /// <summary>
    /// ���� ��ü�Ҷ��� ���Ǵ� DiscardWeapon�޼���
    /// DiscardZone�� ���������� DiscardZone�� �ִ� �޼��� ����
    /// </summary>
    /// <param name="curWeaponId"></param>
    public void DiscardWeapon(int curWeaponId)
    {
        Transform playerTransform = GameManager.Instance.player.transform;
        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * 2f + playerTransform.up * 2f;
        GameObject go = Instantiate(weaponSlots[curWeaponId].GetItemData().itemData.dropPrefab, spawnPosition, Quaternion.identity);
        //5f << �������� ���󰡴� ��
        go.GetComponent<Rigidbody>().AddForce(playerTransform.forward * 5f, ForceMode.Impulse);
        weaponSlots[curWeaponId].ClearSlot();
    }
    #endregion

    #region SortingSystem
    /// <summary>
    /// ������ ����
    /// �Ѿ�, ������, ��, ����ź ������ ����
    /// </summary>
    public void SortInventory()
    {
        List<Item> itemList = new List<Item>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                itemList.Add(slots[i].item);
            }
        }
        //�� ���� ���� �������� ����
        itemList = itemList.OrderBy(item => GetCategoryPriority(item.itemData.type))
            .ThenBy(item => GetCategoryBulletPriority(item.itemData.bulletType))
            .ThenBy(item => GetCategoryHealPriority(item.itemData.healType))
            .ToList();

        //��� ���� �ʱ�ȭ (�� ������ ������ ��ġ�ϱ� ����)
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }

        //���ĵ� �������� �ٽ� �տ������� ä���
        for (int i = 0; i < itemList.Count; i++)
        {
            slots[i].AddItem(itemList[i]);
            slots[i].SetImage();
        }
    }

    /// <summary>
    /// ī�װ� ���� ����
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    private int GetCategoryPriority(Define.ItemType category)
    {
        switch (category)
        {
            case Define.ItemType.bullet: return 1;
            case Define.ItemType.item: return 2;
            case Define.ItemType.heal: return 3;
            case Define.ItemType.grenade: return 4;
            default: return int.MaxValue; // �߸��� ī�װ� ó��
        }
    }

    /// <summary>
    /// �Ѿ� ���� Ÿ������ ����
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    private int GetCategoryBulletPriority(Define.bulletType category)
    {
        switch (category)
        {
            case Define.bulletType.Light: return 1;
            case Define.bulletType.Heavy: return 2;
            case Define.bulletType.Energy: return 3;
            case Define.bulletType.Sniper: return 4;
            case Define.bulletType.Shotgun: return 5;
            default: return int.MaxValue; // �߸��� ī�װ� ó��
        }
    }

    /// <summary>
    /// �� ������ ������ ����
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    private int GetCategoryHealPriority(Define.HealType category)
    {
        switch (category)
        {
            case Define.HealType.shieldBattery: return 1;
            case Define.HealType.shieldCell: return 2;
            case Define.HealType.syringe: return 3;
            case Define.HealType.medikit: return 4;
            default: return int.MaxValue; // �߸��� ī�װ� ó��
        }
    }

    private int GetCategoryGrenadePriority(Define.GrenadeType category)
    {
        switch (category)
        {
            case Define.GrenadeType.fragGrenade: return 1;
            case Define.GrenadeType.thermite: return 1;
            case Define.GrenadeType.arcStar: return 1;
            default: return int.MaxValue; // �߸��� ī�װ� ó��
        }
    }
    #endregion




    public int GetBulletFromInventory(Define.bulletType bulletType, int curBulletCnt, int needMaxBulletCnt)
    {
        if (slots == null) return 0;

        int totalBulletsTaken = 0;
        int bulletsNeeded = needMaxBulletCnt - curBulletCnt;

        if (bulletsNeeded <= 0) return 0;

        foreach (var slot in slots)
        {
            if (slot.item == null) continue;
            if (slot.item.itemData.type != Define.ItemType.bullet) continue;
            if (slot.item.itemData.bulletType != bulletType) continue;

            //������ �� �ִ� �Ѿ� ���
            int bulletsToTake = Mathf.Min(bulletsNeeded, slot.item.quantity);
            slot.item.quantity -= bulletsToTake;
            totalBulletsTaken += bulletsToTake;
            bulletsNeeded -= bulletsToTake;

            slot.UpdateUI();

            if (bulletsNeeded <= 0)
            {
                RefilItem(slot.item);
                break;
            }
        }

        return totalBulletsTaken; // ������ �Ѿ� ���� ��ȯ
    }

    public Item GetGrenadeFromInventory()
    {
        if (slots == null) return null;
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item.itemData.type == Define.ItemType.grenade)
            {
                return slot.item;
            }
        }
        return null;
    }

    public int GetTotalAmmoFromInventory(Define.bulletType bulletType)
    {
        if (slots == null) return 0;
        int totalAmmo = 0;
        foreach (var slot in slots)
        {
            if (slot.item == null) continue;
            if (slot.item.itemData.type != Define.ItemType.bullet) continue;
            if (slot.item.itemData.bulletType != bulletType) continue;

            //������ �� �ִ� �Ѿ� ���
            totalAmmo += slot.item.quantity;
        }
        return totalAmmo;
    }

    #region RefillSystem
    public void RefilItem(Item usedItem)
    {
        if (slots == null) return;

        //������ ���� ������ ���� ���� ����
        RefillItemSlot(usedItem);

        var slotToClear = slots.LastOrDefault(s => s.item == usedItem);
        if (slotToClear != null && usedItem.quantity <= 0)
        {
            slotToClear.ClearSlot();
        }

        SortInventory();
    }

    private void RefillItemSlot(Item targetSlotItem)
    {
        for (int i = slots.Length - 1; i >= 0; i--)
        {
            var slot = slots[i];
            if (slot.item != null && slot.item.itemData == targetSlotItem.itemData && slot.item != targetSlotItem)
            {
                // ���� ������ �������� ����� ���Կ� ����
                int availableToMove = Mathf.Min(slot.item.quantity, targetSlotItem.itemData.maxCount - targetSlotItem.quantity);
                targetSlotItem.quantity += availableToMove;
                slot.item.quantity -= availableToMove;

                slot.UpdateUI();

                // ����� ����Ǿ��ٸ� Ż��
                if (targetSlotItem.quantity == targetSlotItem.itemData.maxCount)
                    break;
            }
        }
    }
}
#endregion

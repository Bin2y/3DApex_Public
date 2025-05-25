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
    //PlayerEquipManager.cs AttachWeaponRoot가 구독
    public UnityEvent<int> OnAcquireWeapon;
    public UnityEvent OnAcquireItem;


    public void Init(Player player)
    {
        slots = slotsParent.GetComponentsInChildren<UI_SubItem_ItemSlot>();
        _player = player;
        SetSlotsParents();
        weaponSlots = weaponSlotsParent.GetComponentsInChildren<UI_SubItem_WeaponSlot>();
        //인벤토리가 Init될때 이벤트 구독
        OnAcquireWeapon.AddListener(_player.playerEquipManager.AttachWeaponOnRoot);
        //weaponSlots 구독시키기 
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
    /// 슬롯들에게 부모님 설정해주기
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
    /// 아이템을 획득하는 코드
    /// </summary>
    /// <param name="item"></param>
    public void AcquireItem(Item item)
    {
        if (slots == null)
        {
            Debug.Log("Slots NULL!!");
            return;
        }

        int remainingQuantity = item.quantity; //남은 아이템 수량

        //슬롯을 한 번만 순회하면서 기존 슬롯 확인 및 빈 슬롯 찾기
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
                emptySlot = slot; //빈 슬롯 찾기
            }
        }
        if (emptySlot == null)
        {
            Debug.Log("Inventory is Full");
            return;
        }

        //남은 아이템이 있다면 빈 슬롯에 추가
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
            //모든 슬롯을 돌았지만 꽉 차있는 경우
            Debug.Log("가지고 있던 무기를 버리고 교체합니다");
            int curWeaponId = _player.fPSController.GetActiveWeaponIndex();
            //현재 weaponSlot은 0,1번 이지만 GetActiveWeaopnIndex메서드는 1,2번으로 리턴함
            ChangeWeapon(item, curWeaponId - 1);
        }
        else
        {
            Debug.Log("Slots NULL!!");
        }
    }

    /// <summary>
    /// 무기 교체 시스템, 무기 버리기 + 무기 세팅 메서드를 둘다 가지고있다.
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="curWeaponId"></param>
    public void ChangeWeapon(Item newItem, int curWeaponId)
    {
        DiscardWeapon(curWeaponId);
        SetWeapon(newItem, curWeaponId);
    }
    /// <summary>
    /// 수류탄 갯수를 반환하는 함수
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
    /// 실제로 수류탄을 던질때
    /// 사용하는 함수
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

                SortInventory(); //정렬
                return;
            }
        }
    }

    /// <summary>
    /// 무기를 배정할 때 사용하는 메서드
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
    /// 무기 교체할때만 사용되는 DiscardWeapon메서드
    /// DiscardZone에 버려질때는 DiscardZone에 있는 메서드 실행
    /// </summary>
    /// <param name="curWeaponId"></param>
    public void DiscardWeapon(int curWeaponId)
    {
        Transform playerTransform = GameManager.Instance.player.transform;
        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * 2f + playerTransform.up * 2f;
        GameObject go = Instantiate(weaponSlots[curWeaponId].GetItemData().itemData.dropPrefab, spawnPosition, Quaternion.identity);
        //5f << 아이템이 날라가는 힘
        go.GetComponent<Rigidbody>().AddForce(playerTransform.forward * 5f, ForceMode.Impulse);
        weaponSlots[curWeaponId].ClearSlot();
    }
    #endregion

    #region SortingSystem
    /// <summary>
    /// 아이템 정렬
    /// 총알, 아이템, 힐, 수류탄 순서로 정렬
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
        //네 가지 정렬 조건으로 정렬
        itemList = itemList.OrderBy(item => GetCategoryPriority(item.itemData.type))
            .ThenBy(item => GetCategoryBulletPriority(item.itemData.bulletType))
            .ThenBy(item => GetCategoryHealPriority(item.itemData.healType))
            .ToList();

        //모든 슬롯 초기화 (빈 슬롯을 앞으로 배치하기 위함)
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }

        //정렬된 아이템을 다시 앞에서부터 채우기
        for (int i = 0; i < itemList.Count; i++)
        {
            slots[i].AddItem(itemList[i]);
            slots[i].SetImage();
        }
    }

    /// <summary>
    /// 카테고리 별로 정렬
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
            default: return int.MaxValue; // 잘못된 카테고리 처리
        }
    }

    /// <summary>
    /// 총알 종류 타입으로 정렬
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
            default: return int.MaxValue; // 잘못된 카테고리 처리
        }
    }

    /// <summary>
    /// 힐 아이템 종류로 정렬
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
            default: return int.MaxValue; // 잘못된 카테고리 처리
        }
    }

    private int GetCategoryGrenadePriority(Define.GrenadeType category)
    {
        switch (category)
        {
            case Define.GrenadeType.fragGrenade: return 1;
            case Define.GrenadeType.thermite: return 1;
            case Define.GrenadeType.arcStar: return 1;
            default: return int.MaxValue; // 잘못된 카테고리 처리
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

            //가져올 수 있는 총알 계산
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

        return totalBulletsTaken; // 가져온 총알 개수 반환
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

            //가져올 수 있는 총알 계산
            totalAmmo += slot.item.quantity;
        }
        return totalAmmo;
    }

    #region RefillSystem
    public void RefilItem(Item usedItem)
    {
        if (slots == null) return;

        //마지막 슬롯 아이템 보충 로직 실행
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
                // 현재 슬롯의 아이템을 사용한 슬롯에 보충
                int availableToMove = Mathf.Min(slot.item.quantity, targetSlotItem.itemData.maxCount - targetSlotItem.quantity);
                targetSlotItem.quantity += availableToMove;
                slot.item.quantity -= availableToMove;

                slot.UpdateUI();

                // 충분히 보충되었다면 탈출
                if (targetSlotItem.quantity == targetSlotItem.itemData.maxCount)
                    break;
            }
        }
    }
}
#endregion

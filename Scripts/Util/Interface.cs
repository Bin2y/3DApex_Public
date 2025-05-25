/// <summary>
/// 상호작용가능한 오브젝트에 붙혀준다.
/// </summary>
public interface IInteractable
{
    void OnInteract(Player player);
    void OnInteractWeapon(Player player);
    //현재 아이템 타입을 반환
    Define.ItemType GetItemType();
    public ItemDataSO GetInteractPrompt();
}

/// <summary>
/// 데미지를 입는 객체에 붙혀준다.
/// </summary>
public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);
}

/// <summary>
/// WeaponSlot, ItemSlot에 필요한 슬롯 인터페이스
/// 해당 인터페이스로 DragSlot이 작동
/// </summary>
public interface ISlot
{
    public void ClearSlot();
    public Item GetItemData();
    public void UpdateUI();
    public void AddItem(Item item);
    //우클릭이나 무기 교체시에 사용되는 메서드
}
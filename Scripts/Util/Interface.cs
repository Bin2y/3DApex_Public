/// <summary>
/// ��ȣ�ۿ밡���� ������Ʈ�� �����ش�.
/// </summary>
public interface IInteractable
{
    void OnInteract(Player player);
    void OnInteractWeapon(Player player);
    //���� ������ Ÿ���� ��ȯ
    Define.ItemType GetItemType();
    public ItemDataSO GetInteractPrompt();
}

/// <summary>
/// �������� �Դ� ��ü�� �����ش�.
/// </summary>
public interface IDamageable
{
    bool ApplyDamage(DamageMessage damageMessage);
}

/// <summary>
/// WeaponSlot, ItemSlot�� �ʿ��� ���� �������̽�
/// �ش� �������̽��� DragSlot�� �۵�
/// </summary>
public interface ISlot
{
    public void ClearSlot();
    public Item GetItemData();
    public void UpdateUI();
    public void AddItem(Item item);
    //��Ŭ���̳� ���� ��ü�ÿ� ���Ǵ� �޼���
}
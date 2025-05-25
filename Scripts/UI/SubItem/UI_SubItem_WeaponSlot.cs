using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_SubItem_WeaponSlot : UI_SubItem, ISlot, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Image")]
    public Image weaponImage;
    //�Ѿ�����
    public Image bulletImage;

    [Header("TMP")]
    public TextMeshProUGUI bulletCntTmp;
    [Header("Item")]
    [SerializeField] public Item item;
    [SerializeField] public Gun gun;

    [Header("WeaponSlotId")]
    [SerializeField] private int weaponSlotId;

    //���� ����ִ� ���Ⱑ �ش� �������� Ȯ���ϴ� ����
    public bool isActive = false;

    //���Ⱑ �������� �̺�Ʈ => Player���� �������Ѽ� ������Ʈ �ı��ؾ���
    public UnityEvent<int> OnDiscardWeapon;
    public UnityEvent<int> OnChangeWeaponSlot;

    //�÷��̾� ����
    private Player player;

    public void Init(Player player)
    {
        this.player = player;
    }
    protected override void OnEnable()
    {
        SetBulletCntText();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.curSlot = this;
            DragSlot.instance.DragSetImage(weaponImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.curSlot = null;
    }
    public void OnDrop(PointerEventData eventData)
    {
        ChangeWeaponSlot();
    }
    private void ChangeWeaponSlot()
    {
        if (DragSlot.instance.curSlot == null) return;
        Item tempItem = item;

        AddItem(DragSlot.instance.curSlot.GetItemData());

        if (tempItem != null)
        {
            DragSlot.instance.curSlot.AddItem(tempItem);
            DragSlot.instance.curSlot.UpdateUI();
        }
        else
        {
            DragSlot.instance.curSlot.ClearSlot();
        }
        OnChangeWeaponSlot?.Invoke(GetWeaponSlotId());
        UpdateUI();
    }

    public void AddItem(Item item)
    {
        this.item = item;
        UpdateUI();
    }
    public void UpdateUI()
    {
        SetImage();
    }

    public void SetImage()
    {
        if (item == null) { return; }
        weaponImage.sprite = item.itemData.icon;
        bulletImage.sprite = item.itemData.bulletIcon;
        SetColor();
    }

    public void SetColor()
    {
        Color color = Color.white;
        if (item == null)
        {
            color.a = 0;
            weaponImage.color = color;
            bulletImage.color = color;
            return;
        }
        color.a = 1f;
        weaponImage.color = color;
        bulletImage.color = color;
    }

    //OnEnable(�κ��丮�� ������) ���� ����
    public void SetBulletCntText()
    {
        if (gun == null) return;
        bulletCntTmp.text = gun.currentAmmo.ToString();
    }

    public void ClearSlot()
    {
        //�̹����� ���� ����.
        item = null;
        weaponImage.sprite = null;
        bulletImage.sprite = null;
        Color color = Color.white;
        color.a = 0;
        weaponImage.color = color;
        bulletImage.color = color;
        isActive = false;
        //tmp ����
        bulletCntTmp.text = null;
        //���� ���� Gun ����
        gun = null;
        //���� ������ ���� Ŭ������ �� �̺�Ʈ �߻�
        OnDiscardWeapon?.Invoke(GetWeaponSlotId());
        UpdateInGameWeaponUI();
    }

    public int GetWeaponSlotId()
    {
        return weaponSlotId;
    }

    public Item GetItemData()
    {
        return item;
    }

    #region UI_SubItem_Weapon �Լ�
    //�ΰ��� weaponUI�� �������ִ� wrapping �޼���
    //�ش� �޼���� AttachWeapon�� ȣ���Ѵ�.
    //Attach�ϱ������� Gun�� Null��
    public void SetInGameWeaponUI()
    {
        player.playerUI.inGameUI.ui_weapon.SetInGameWeaponSlot(item, gun, GetWeaponSlotId());
    }

    public void UpdateInGameWeaponUI()
    {
        player.playerUI.inGameUI.ui_weapon.Clear(GetWeaponSlotId());
    }
    #endregion
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_SubItem_WeaponSlot : UI_SubItem, ISlot, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Image")]
    public Image weaponImage;
    //총알종류
    public Image bulletImage;

    [Header("TMP")]
    public TextMeshProUGUI bulletCntTmp;
    [Header("Item")]
    [SerializeField] public Item item;
    [SerializeField] public Gun gun;

    [Header("WeaponSlotId")]
    [SerializeField] private int weaponSlotId;

    //현재 들고있는 무기가 해당 슬롯인지 확인하는 변수
    public bool isActive = false;

    //무기가 버려지는 이벤트 => Player에게 구독시켜서 오브젝트 파괴해야함
    public UnityEvent<int> OnDiscardWeapon;
    public UnityEvent<int> OnChangeWeaponSlot;

    //플레이어 참조
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

    //OnEnable(인벤토리가 열릴때) 마다 갱신
    public void SetBulletCntText()
    {
        if (gun == null) return;
        bulletCntTmp.text = gun.currentAmmo.ToString();
    }

    public void ClearSlot()
    {
        //이미지를 전부 비운다.
        item = null;
        weaponImage.sprite = null;
        bulletImage.sprite = null;
        Color color = Color.white;
        color.a = 0;
        weaponImage.color = color;
        bulletImage.color = color;
        isActive = false;
        //tmp 비우기
        bulletCntTmp.text = null;
        //현재 슬롯 Gun 비우기
        gun = null;
        //웨폰 슬롯은 슬롯 클리어할 때 이벤트 발생
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

    #region UI_SubItem_Weapon 함수
    //인게임 weaponUI를 갱신해주는 wrapping 메서드
    //해당 메서드는 AttachWeapon때 호출한다.
    //Attach하기전에는 Gun이 Null임
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

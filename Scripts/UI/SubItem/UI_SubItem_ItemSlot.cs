using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SubItem_ItemSlot : UI_SubItem, ISlot, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Image")]
    public Image itemImage;

    [Header("Button")]
    public Button button;

    [Header("TMP")]
    public TextMeshProUGUI itemCntTmp;

    [Header("Item Data")]
    public Item item;

    //부모 인벤토리를 슬롯도 소유
    private UI_Popup_Inventory inventory;

    private Player player;

    public void Init(UI_Popup_Inventory inventory, Player player)
    {
        this.inventory = inventory;
        this.player = player;   
    }
    /// <summary>
    /// 받은 아이템 데이터 이미지를 넣어준다.
    /// </summary>
    public void SetImage()
    {
        if (item.itemData == null) { return; }
        itemImage.sprite = item.itemData.icon;
        SetColor();
    }

    public void SetColor()
    {
        Color color = Color.white;
        if (item.itemData == null)
        {
            color.a = 0;
            itemImage.color = color;
            return;
        }
        color.a = 1f;
        itemImage.color = color;
    }

    public void AddItem(Item item)
    {
        this.item = item;
        UpdateUI();
    }

    /// <summary>
    /// 이벤트를 연결해줌
    /// </summary>
    /*public void AddOnclickEvent()
    {
        button.onClick.AddListener(UseItem);
    }

    public void RemoveOnclickEvent()
    {
        button.onClick.RemoveListener(UseItem);
    }*/

    public void SetItemCntText()
    {
        itemCntTmp.text = item.quantity.ToString();
    }

    #region DragHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;
        if (!item.UseItem(player)) return;
        item.quantity -= 1;
        inventory.RefilItem(item);
        UpdateUI();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;
        if (item.itemData != null)
        {
            DragSlot.instance.curSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;
        if (item.itemData != null)
            DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.curSlot = null;
    }




    //OnDrop이벤트에 넣어줄 메서드
    /*private void ChangeItemSlot()
    {
        if (DragSlot.instance.curSlot == null) return;
        Item tempItem = item;
        Item dragItem = DragSlot.instance.curSlot.GetItemData();

        DragSlot.instance.curSlot.AddItem(tempItem);
        DragSlot.instance.curSlot.UpdateUI();

        this.AddItem(dragItem);
        this.UpdateUI();

        *//*if (tempItem != null)
        {
            DragSlot.instance.curSlot.AddItem(item);
            DragSlot.instance.curSlot.UpdateUI();
        }
        else
        {
            DragSlot.instance.curSlot.ClearSlot();
        }*//*
        //UpdateUI();
    }*/

    /// <summary>
    /// 슬롯을 리프레쉬해준다.
    /// 데이터는 이미 바뀌었으니
    /// 바뀐 데이터로 UI를 업데이트
    /// </summary>
    public void UpdateUI()
    {
        if (item == null || item.quantity <= 0)
        {
            ClearSlot();
            inventory.SortInventory();
        }
        else
        {
            SetImage();
            SetItemCntText();
        }
    }

    /// <summary>
    /// 슬롯을 비운다
    /// </summary>
    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        Color color = Color.white;
        color.a = 0;
        itemImage.color = color;
        itemCntTmp.text = null;
    }

    public Item GetItemData()
    {
        return item;
    }
    #endregion
}

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

    //�θ� �κ��丮�� ���Ե� ����
    private UI_Popup_Inventory inventory;

    private Player player;

    public void Init(UI_Popup_Inventory inventory, Player player)
    {
        this.inventory = inventory;
        this.player = player;   
    }
    /// <summary>
    /// ���� ������ ������ �̹����� �־��ش�.
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
    /// �̺�Ʈ�� ��������
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




    //OnDrop�̺�Ʈ�� �־��� �޼���
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
    /// ������ �����������ش�.
    /// �����ʹ� �̹� �ٲ������
    /// �ٲ� �����ͷ� UI�� ������Ʈ
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
    /// ������ ����
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

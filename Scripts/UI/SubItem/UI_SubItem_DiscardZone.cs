using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class UI_SubItem_DiscardZone : UI_SubItem, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private UI_Popup_Inventory inventory;
    public Image bgImage;
    public void Init(UI_Popup_Inventory inventory)
    {
        this.inventory = inventory;
        bgImage = GetComponent<Image>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance == null) return;
        if (DragSlot.instance != null)
        {
            // 드랍 영역에 놓였을 때 아이템 삭제 처리
            DragSlot.instance.DiscardItem();
            inventory.SortInventory();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragSlot.instance.curSlot != null)
        {
            for (int i = 0; i < inventory.discardZone.Length; i++)
            {
                Color color = Color.white;
                color.a = 0.75f;
                inventory.discardZone[i].bgImage.color = color;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragSlot.instance != null)
        {
            for (int i = 0; i < inventory.discardZone.Length; i++)
            {
                Color color = Color.white;
                color.a = 0f;
                inventory.discardZone[i].bgImage.color = color;
            }
        }
    }
}

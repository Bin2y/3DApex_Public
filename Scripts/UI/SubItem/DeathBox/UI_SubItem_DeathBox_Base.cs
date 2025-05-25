using UnityEngine.EventSystems;
using UnityEngine;

public enum ItemCategory { Inventory, Weapon, Bullet }

public abstract class UI_SubItem_DeathBox_Base : UI_SubItem, IPointerClickHandler
{
    [HideInInspector] public UI_Popup_DeathBox parentUI;
    protected DeathBox model;
    protected ItemCategory category;
    protected int index;

    /// <summary>
    /// ui가 DeathBox 모델에 바인딩될 때 호출
    /// </summary>
    public virtual void Bind(DeathBox deathBox, ItemCategory cat, int idx)
    {
        model = deathBox;
        category = cat;
        index = idx;
        UpdateUI();
    }

    public abstract void UpdateUI();

    public void OnPointerClick(PointerEventData eventData)
    {
        Item item = category switch
        {
            ItemCategory.Inventory => model.items[index],
            ItemCategory.Weapon => model.weapons[index],
            ItemCategory.Bullet => model.bullets[index],
            _ => null
        };

        if (item == null) return;
 
        switch (category)
        {
            case ItemCategory.Inventory:
                parentUI.player.playerInventoryController.inventory.AcquireItem(new Item(item.itemData, item.quantity));
                //model.items.RemoveAt(index);
                model.items[index] = null;
                break;
            case ItemCategory.Weapon:
                parentUI.player.playerInventoryController.inventory.AcquireWeapon(new Item(item.itemData, item.quantity));
                //model.weapons.RemoveAt(index);
                model.weapons[index] = null;
                break;
            case ItemCategory.Bullet:
                parentUI.player.playerInventoryController.inventory.AcquireItem(new Item(item.itemData, item.quantity));
                //model.bullets.RemoveAt(index);
                model.bullets[index] = null;
                break;
        }
        //ui 리프레시
        parentUI.Refresh();
        UpdateUI();
    }

    public virtual void Clear()
    {
        model = null;
        category = 0;
        index = -1;
        //ui 초기화 
    }
}
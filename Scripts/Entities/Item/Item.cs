using System;
using UnityEngine;
using UnityEngine.Events;

public static class ItemEvents
{
    public static Func<Item, bool> OnItemUsed;
}
public class Item
{
    [Header("Item Data")]
    public ItemDataSO itemData;
    public int quantity;


    public Item(ItemDataSO itemData, int quantity = 1)
    {
        this.itemData = itemData;
        this.quantity = quantity;
    }

    /// <summary>
    /// Consumable 아이템 사용
    /// 현재는 힐팩, 쉴드 아이템
    /// </summary>
    /// <returns></returns>
    /// 
    public bool UseItem(Player player)
    {
        if (!itemData.isConsumable) return false;
        bool canUse = ItemEvents.OnItemUsed?.Invoke(this) ?? false;
        player.fPSController._actionState = FPSActionState.UsingItem;
        return canUse;
    }

    
}
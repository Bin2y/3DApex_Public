using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct LootEntry
{
    public ItemDataSO data;
    public int quantity;
}

public class DummyLoot : MonoBehaviour
{
    [Header("소비 아이템으로 들어갈 목록")]
    public List<LootEntry> inventoryLoot;

    [Header("무기로 들어갈 목록")]
    public List<LootEntry> weaponLoot;

    [Header("총알 아이템 목록")]
    public List<LootEntry> bulletLoot;

    public List<Item> GetInventoryItems()
        => inventoryLoot.Select(e => new Item(e.data, e.quantity)).ToList();

    public List<Item> GetWeaponItems()
        => weaponLoot.Select(e => new Item(e.data, e.quantity)).ToList();

    public List<Item> GetBulletItems()
        => bulletLoot.Select(e => new Item(e.data, e.quantity)).ToList();
}
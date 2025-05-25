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
    [Header("�Һ� ���������� �� ���")]
    public List<LootEntry> inventoryLoot;

    [Header("����� �� ���")]
    public List<LootEntry> weaponLoot;

    [Header("�Ѿ� ������ ���")]
    public List<LootEntry> bulletLoot;

    public List<Item> GetInventoryItems()
        => inventoryLoot.Select(e => new Item(e.data, e.quantity)).ToList();

    public List<Item> GetWeaponItems()
        => weaponLoot.Select(e => new Item(e.data, e.quantity)).ToList();

    public List<Item> GetBulletItems()
        => bulletLoot.Select(e => new Item(e.data, e.quantity)).ToList();
}
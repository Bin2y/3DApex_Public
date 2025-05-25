using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour, IInteractable
{
    public ItemDataSO deatBoxDataSO;

    [SerializeField] public List<Item> items;
    [SerializeField] public List<Item> weapons;
    [SerializeField] public List<Item> bullets;

    public float closeDistance = 5f;

    public void Init(List<Item> inventoryItems, List<Item> weaponItems, List<Item> bulletItems)
    {
        items = inventoryItems;
        weapons = weaponItems;
        bullets = bulletItems;
    }
    public ItemDataSO GetInteractPrompt()
    {
        return deatBoxDataSO;
    }

    public Define.ItemType GetItemType()
    {
        return deatBoxDataSO.type;
    }

    public void OnInteract(Player player)
    {
        player.playerUI.deathBoxUI.Bind(player, this);
    }

    public void OnInteractWeapon(Player player)
    {
        throw new System.NotImplementedException();
    }
}

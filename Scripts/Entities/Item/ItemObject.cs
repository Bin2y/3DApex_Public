using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [Header("Item")]
    public ItemDataSO item;
    public int quantity;



    public ItemDataSO GetInteractPrompt()
    {
        return item;
    }

    public void OnInteractWeapon(Player _player)
    {
        _player.playerInventoryController.inventory.AcquireWeapon(new Item(item));
        Destroy(gameObject);
    }

    public void OnInteract(Player _player)
    {
        _player.playerInventoryController.inventory.AcquireItem(new Item(item, quantity));

        //Destroy(gameObject);
    }

    //���� Interact�ϰ��ִ� ������ Ÿ���� ����
    public Define.ItemType GetItemType()
    {
        return item.type;
    }
}

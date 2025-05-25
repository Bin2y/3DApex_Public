using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_DeathBox_ItemSlot : UI_SubItem_DeathBox_Base
{
    [Header("UI")]
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public override void UpdateUI()
    {
        if (model == null || model.items[index] == null)
        {
            itemIcon.enabled = false;
            itemName.enabled = false;
            return;
        }

        var itm = model.items[index];
        itemIcon.enabled = true;
        itemName.enabled = true;


        itemIcon.sprite = itm.itemData.icon;
        itemName.text = itm.itemData.itemName;
    }
}

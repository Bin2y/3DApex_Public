using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_DeathBox_WeaponSlot : UI_SubItem_DeathBox_Base
{
    [Header("UI")]
    public Image weaponIcon;
    public Image bulletTypeIcon;
    public TextMeshProUGUI weaponName;
    public override void UpdateUI()
    {
        if (model == null || model.weapons[index] == null)
        {
            weaponIcon.enabled = false;
            bulletTypeIcon.enabled = false;
            weaponName.enabled = false;
            return;
        }

        var itm = model.weapons[index];
        weaponIcon.enabled = true;
        bulletTypeIcon.enabled = true;
        weaponName.enabled = true;

        weaponIcon.sprite = itm.itemData.icon;
        bulletTypeIcon.sprite = itm.itemData.bulletIcon;
        weaponName.text = itm.itemData.itemName;

    }
}

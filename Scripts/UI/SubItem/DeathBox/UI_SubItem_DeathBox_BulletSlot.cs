using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_DeathBox_BulletSlot : UI_SubItem_DeathBox_Base
{ 

    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemCntTmp;



    public override void UpdateUI()
    {
        //if (model == null || index >= model.bullets.Count)
        if(model == null || model.bullets[index] == null)
        {
            //icon.enabled = false;
            itemCntTmp.text = "--";
            return;
        }

        var itm = model.bullets[index];
        icon.enabled = true;
        icon.sprite = itm.itemData.icon;
        itemCntTmp.text = itm.quantity.ToString();
    }
}

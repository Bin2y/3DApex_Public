using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] public UI_Scene_InGame inGameUI;
    [SerializeField] public UI_Popup_Inventory inventoryUI;
    [SerializeField] public UI_Popup_HealSlot healSlotUI;
    [SerializeField] public UI_Popup_GrenadeSlot grenadeSlotUI;
    [SerializeField] public UI_Popup_DeathBox deathBoxUI;

    public void Init(Player player)
    {
        inGameUI.Init(player);
        healSlotUI.Init(player);
        grenadeSlotUI.Init(player);
    }
}

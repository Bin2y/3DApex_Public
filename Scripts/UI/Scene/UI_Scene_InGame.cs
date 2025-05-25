

public class UI_Scene_InGame : UI_Scene
{
    public UI_SubItem_HPShieldBar ui_HPShieldBar;
    public UI_SubItem_Interact ui_Interact;
    public UI_SubItem_CrossHair ui_crossHair;
    public UI_SubItem_UsingItem ui_usingItem;
    public UI_SubItem_Weapon ui_weapon;
    public UI_SubItem_Heal ui_heal;
    public UI_SubItem_Grenade ui_grenade;
    public UI_SubItem_Abliity ui_abliity;

    public void Init(Player player)
    {
        ui_weapon.Init(player);
        ui_heal.Init(player);
        ui_grenade.Init(player);
        ui_abliity.Init(player);
    }
}

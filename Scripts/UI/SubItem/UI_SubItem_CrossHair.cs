using UnityEngine;

public class UI_SubItem_CrossHair : UI_SubItem
{
    public GameObject crossHair_Fist;
    public GameObject crossHair_Weapon;

    public void ActiveCrossHairWeapon()
    {
        crossHair_Fist.SetActive(true);
    }

    public void DeActiveCrossHairWeapon()
    {
        crossHair_Fist.SetActive(false);
    }
}

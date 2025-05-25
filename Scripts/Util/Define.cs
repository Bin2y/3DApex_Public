using UnityEngine;

/// <summary>
/// ���� ���ǵ��� ��Ƴ��� Ŭ����
/// </summary>
public static class Define
{

    /// <summary>
    /// ������ �޼��� ����ü
    /// </summary>
    

    /// <summary>
    /// �Ѿ� Ÿ��
    /// </summary>
    public enum bulletType
    {
        None,
        Light,
        Heavy,
        Energy,
        Sniper,
        Shotgun
    }
    /// <summary>
    /// ������ Ÿ��
    /// </summary>
    public enum ItemType
    {
        bullet,
        item,
        heal,
        grenade,
        weapon,
        DeathBox
    }

    public enum BodyArmor
    {
        None,
        Tier_1,
        Tier_2,
        Tier_3,
    }

    public enum HealType
    {
        None,
        shieldCell,
        shieldBattery,
        syringe,
        medikit
    }

    public enum GrenadeType
    {
        None,
        fragGrenade,
        thermite,
        arcStar
    }

    public enum WeaponType
    {
        None,
        melee,
        grenade,
        gun
    }


}

public struct DamageMessage
{
    public GameObject damager;
    public int amount;

    public Vector3 hitPoint;
    public Vector3 hitNormal;
}
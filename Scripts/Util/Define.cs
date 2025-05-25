using UnityEngine;

/// <summary>
/// 여러 정의들을 모아놓은 클래스
/// </summary>
public static class Define
{

    /// <summary>
    /// 데미지 메세지 구조체
    /// </summary>
    

    /// <summary>
    /// 총알 타입
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
    /// 아이템 타입
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
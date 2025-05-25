using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public BodyArmorSystem armorSystem;
    [Header("Shield")]
    public int curShield;
    public const int SHIELD_AMOUNT_PER_SEGMENT = 25;
    [Header("Health")]
    public const int MAX_HEALTH = 100;
    public int curHealth;
    public bool isInvincibility = false;

    /// <summary>
    /// Death event
    /// TakeDamage event
    /// Delegate
    /// </summary>
    public UnityEvent OnDeath;
    public UnityEvent OnTakeDamage;
    public UnityEvent OnHealthShieldChanaged;

    //더미용 Init
    public void Init()
    {
        armorSystem = new BodyArmorSystem();
        curHealth = MAX_HEALTH;
        curShield = CalculateMaxShieldAmount(armorSystem.GetEquippedBodyArmor());
    }
    //플레이어용 Init
    public void Init(Player player)
    {
        armorSystem = new BodyArmorSystem(player);
        curHealth = MAX_HEALTH;
        curShield = CalculateMaxShieldAmount(armorSystem.GetEquippedBodyArmor());
    }

    //overloading
    /*public virtual void Init(int health, int shield)
    {
        maxHealth = health;
        curShield = shield;
    }*/

    public int CalculateMaxShieldAmount(Define.BodyArmor bodyArmor)
    {
        switch (bodyArmor)
        {
            case Define.BodyArmor.None: return 0;
            case Define.BodyArmor.Tier_1: return SHIELD_AMOUNT_PER_SEGMENT * 2;
            case Define.BodyArmor.Tier_2: return SHIELD_AMOUNT_PER_SEGMENT * 3;
            case Define.BodyArmor.Tier_3: return SHIELD_AMOUNT_PER_SEGMENT * 4;
            default: return 0;
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage < curShield)
        {
            AudioManager.Instance.PlaySFX(SoundClips.SFX.Hit_Shield);
            curShield -= damage;
        }
        else
        {
            if (curShield > 0) AudioManager.Instance.PlaySFX(SoundClips.SFX.Shield_Break);
            else AudioManager.Instance.PlaySFX(SoundClips.SFX.Hit_Flesh);
            curHealth -= damage - curShield;
            curShield = 0;
        }
        OnTakeDamage?.Invoke();
        OnHealthShieldChanaged?.Invoke();
        //죽으면 체력 0 고정 및 이벤트 발생
        if (curHealth <= 0)
        {
            curHealth = 0;
            OnDeath?.Invoke();
            isInvincibility = true;
            return;
        }
        Debug.Log("TakeDamage!!");
    }

    public void HealHealth(int amount)
    {
        //Medikit
        if (amount == 100) AudioManager.Instance.PlaySFX(SoundClips.SFX.Use_Medikit);
        //Syringe
        //else if (amount == 25) AudioManager.Instance.PlaySFX(SoundClips.SFX.Use_Syringe);
        curHealth += amount;
        curHealth = Mathf.Clamp(curHealth, 0, MAX_HEALTH);
        OnHealthShieldChanaged?.Invoke();
        Debug.Log("HealHealth!!");
    }

    public void HealShield(int amount)
    {
        curShield += amount;
        curShield = Mathf.Clamp(curShield, 0, CalculateMaxShieldAmount(armorSystem.bodyArmor));
        OnHealthShieldChanaged?.Invoke();
        Debug.Log("HealShield!!");
    }

    public int GetShieldAmountPerSegment()
    {
        return SHIELD_AMOUNT_PER_SEGMENT;
    }
    public int GetMaxHealth()
    {
        return MAX_HEALTH;
    }

    public int GetCurHealth()
    {
        return curHealth;
    }

    public int GetCurShield()
    {
        return curShield;
    }

    protected virtual void Die()
    {
        OnDeath.Invoke();
    }

    public bool IsShieldFull()
    {
        return curShield == CalculateMaxShieldAmount(armorSystem.bodyArmor);
    }

    public bool IsHealthFull()
    {
        return curHealth == MAX_HEALTH;
    }
}

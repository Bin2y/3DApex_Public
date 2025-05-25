using System.Collections;
using UnityEngine;

/// <summary>
/// 훈련 봇
/// </summary>
public class Dummy : MonoBehaviour, IDamageable
{
    [Header("DeathBox")]
    public GameObject deathBoxPrefab;
    public DummyLoot loot;

    [Header("Component")]
    public HealthSystem healthSystem;
    public UI_SubItem_HPShieldBar _UI_SubItem_HPShieldBar;
    public DG_UI_FadeEffect fadeEffect;
    public WaitForSeconds popup_ui_time= new WaitForSeconds(2f);
    public CapsuleCollider colider;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        _UI_SubItem_HPShieldBar = GetComponentInChildren<UI_SubItem_HPShieldBar>();
        fadeEffect = GetComponentInChildren<DG_UI_FadeEffect>();
        loot = GetComponentInChildren<DummyLoot>();
        colider = GetComponent<CapsuleCollider>();
    }
    private void OnEnable()
    {
        healthSystem.Init();
        healthSystem.OnDeath.AddListener(Die);
        _UI_SubItem_HPShieldBar.Init(healthSystem);
    }

    Coroutine PopuphealthBarCoroutine;
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        if (healthSystem.isInvincibility) return true;
        healthSystem.TakeDamage(damageMessage.amount);
        if (PopuphealthBarCoroutine != null)
            StopCoroutine(PopuphealthBarCoroutine);
        PopuphealthBarCoroutine = StartCoroutine(PopupHealthABarUI());
        return true;
    }

    private IEnumerator PopupHealthABarUI()
    {
        fadeEffect.FadeIn();
        yield return popup_ui_time;
        fadeEffect.FadeOut();
    }

    public void Die()
    {
        //데스박스 생성
        GameObject deathBoxGo = Instantiate(deathBoxPrefab, transform.position, Quaternion.identity);

        var deathBox = deathBoxGo.GetComponent<DeathBox>();
        deathBox.Init(loot.GetInventoryItems(), loot.GetWeaponItems(), loot.GetBulletItems());
        //콜라이더 끄기
        colider.enabled = false;
        
        //체력바
        StopCoroutine(PopuphealthBarCoroutine);
        PopuphealthBarCoroutine= null;
        StartCoroutine(DieCoroutine());
    }
    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

}

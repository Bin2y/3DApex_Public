using System.Collections;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    public Player player;

    public bool isUsingItem = false;
    public void Init()
    {

        player = GetComponent<Player>();
        if (player == null) Debug.Log("플레이어가 존재하지않음");
        ItemEvents.OnItemUsed += HandleItemUse;
        if (ItemEvents.OnItemUsed == null) Debug.Log("아이템 이벤트가 존재하지않음");
    }

    public bool HandleItemUse(Item item)
    {
        if (isUsingItem) return false;
        //이미 체력이 풀이면 return;
        switch (item.itemData.healType)
        {
            case Define.HealType.shieldCell:
                if (player.healthSystem.IsShieldFull()) return false;
                break;
            case Define.HealType.shieldBattery:
                if (player.healthSystem.IsShieldFull()) return false;
                break;
            case Define.HealType.syringe:
                if (player.healthSystem.IsHealthFull()) return false;
                break;
            case Define.HealType.medikit:
                if (player.healthSystem.IsHealthFull()) return false;
                break;
        }
        StartCoroutine(ConsumeItemCoroutine(item));
        return true;
    }

    //아이템에 적혀잇는 ConsumeTime에 따른 코루틴실행
    public IEnumerator ConsumeItemCoroutine(Item item)
    {
        isUsingItem = true;

        UI_SubItem_UsingItem uiUsingItem = player.playerUI.inGameUI.ui_usingItem;

        float consumeTime = item.itemData.consumeTime;
        float timer = consumeTime;

        uiUsingItem.SetUI(item);
        //사운드 적용
        ApplySound(item);

        //UI 코루틴 실행
        while (timer >= 0f)
        {
            timer -= Time.deltaTime;
            uiUsingItem.SetItemTImer(timer / consumeTime, timer);
            yield return null;
        }
        //실제 힐 적용
        ApplyHealing(item);
        player.fPSController.ResetActionState();
        uiUsingItem.ResetTimer();
        isUsingItem = false;
    }

    private void ApplyHealing(Item item)
    {
        switch (item.itemData.healType)
        {
            case Define.HealType.shieldCell:
                if (!player.healthSystem.IsShieldFull())
                    player.healthSystem.HealShield(25);
                break;
            case Define.HealType.shieldBattery:
                if (!player.healthSystem.IsShieldFull())
                    player.healthSystem.HealShield(100);
                break;
            case Define.HealType.syringe:
                if (!player.healthSystem.IsHealthFull())
                    player.healthSystem.HealHealth(25);
                break;
            case Define.HealType.medikit:
                if (!player.healthSystem.IsHealthFull())
                    player.healthSystem.HealHealth(100);
                break;
        }
    }

    private void ApplySound(Item item)
    {
        switch (item.itemData.healType)
        {
            case Define.HealType.shieldCell:
                AudioManager.Instance.PlaySFX(SoundClips.SFX.Use_ShieldCell);
                break;
            case Define.HealType.shieldBattery:
                AudioManager.Instance.PlaySFX(SoundClips.SFX.Use_ShieldBattery);
                break;
            case Define.HealType.syringe:
                AudioManager.Instance.PlaySFX(SoundClips.SFX.Use_Syringe);
                break;
            case Define.HealType.medikit:
                AudioManager.Instance.PlaySFX(SoundClips.SFX.Use_Medikit);
                break;
        }
    }
}

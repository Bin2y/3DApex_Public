using System.Collections;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    public Player player;

    public bool isUsingItem = false;
    public void Init()
    {

        player = GetComponent<Player>();
        if (player == null) Debug.Log("�÷��̾ ������������");
        ItemEvents.OnItemUsed += HandleItemUse;
        if (ItemEvents.OnItemUsed == null) Debug.Log("������ �̺�Ʈ�� ������������");
    }

    public bool HandleItemUse(Item item)
    {
        if (isUsingItem) return false;
        //�̹� ü���� Ǯ�̸� return;
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

    //�����ۿ� �����մ� ConsumeTime�� ���� �ڷ�ƾ����
    public IEnumerator ConsumeItemCoroutine(Item item)
    {
        isUsingItem = true;

        UI_SubItem_UsingItem uiUsingItem = player.playerUI.inGameUI.ui_usingItem;

        float consumeTime = item.itemData.consumeTime;
        float timer = consumeTime;

        uiUsingItem.SetUI(item);
        //���� ����
        ApplySound(item);

        //UI �ڷ�ƾ ����
        while (timer >= 0f)
        {
            timer -= Time.deltaTime;
            uiUsingItem.SetItemTImer(timer / consumeTime, timer);
            yield return null;
        }
        //���� �� ����
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

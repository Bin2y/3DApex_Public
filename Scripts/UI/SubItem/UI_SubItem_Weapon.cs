using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_Weapon : MonoBehaviour
{
    [Header("Image")]
    public Image bulletIcon;
    public Image weaponIcon;
    [Header("Tmp")]
    public TextMeshProUGUI inventoryBulletTmp;
    public TextMeshProUGUI curBulletTmp;
    public TextMeshProUGUI weaponTmp1;
    public TextMeshProUGUI weaponTmp2;
    [Header("GameObject")]
    public GameObject toggleWeaponSlot1;
    public GameObject toggleWeaponSlot2;


    private Player player;
    private int _activeWeaponSlotIdx;

    private Item[] item = new Item[2];
    private Gun[] gun = new Gun[2];


    public void Init(Player player)
    {
        this.player = player;
        //아이템을 습득할때마다 해당 슬롯에 맞는 무기의 Bullet을 UI에 반영
        player.playerUI.inventoryUI.OnAcquireItem.AddListener(() => SetInventoryAmmoTmp(_activeWeaponSlotIdx));
    }
    //무기를 장착할떄 호출되는 메서드 Wrapping 중
    public void SetInGameWeaponSlot(Item item, Gun gun, int weaponSlotidx)
    {
        SetItemAndGun(item, gun, weaponSlotidx);
        ConnectGunEvent(weaponSlotidx);
        SetImage(weaponSlotidx) ;
        ActiveWeaponSlot(weaponSlotidx);
        InitTmp(weaponSlotidx);
    }

    public void SetItemAndGun(Item item, Gun gun,  int weaponSlotIdx)
    {
        this.item[weaponSlotIdx] = item;
        this.gun[weaponSlotIdx] = gun;
    }
    //Sprite 세팅
    public void SetImage(int weaponSlotidx)
    {
        if (item == null) { return; }
        weaponIcon.enabled = true;
        bulletIcon.enabled = true;
        weaponIcon.sprite = item[weaponSlotidx].itemData.icon;
        bulletIcon.sprite = item[weaponSlotidx].itemData.bulletIcon;
        SetColor();
    }

    public void SetColor()
    {
        Color color = Color.white;
        if (item == null)
        {
            color.a = 0;
            weaponIcon.color = color;
            bulletIcon.color = color;
            return;
        }
        color.a = 1f;
        weaponIcon.color = color;
        bulletIcon.color = color;
    }
    #region Connect Or Disconnect GunEvent
    public void ConnectGunEvent(int weaponSlotidx)
    {
        gun[weaponSlotidx].OnFireEvent.AddListener(() => SetCurAmmoTmp(weaponSlotidx));
        gun[weaponSlotidx].OnReloadEvent.AddListener(() => SetInventoryAmmoTmp(weaponSlotidx));

    }
    public void DisConnectGunEvent(int weaponSlotidx)
    {
        gun[weaponSlotidx].OnFireEvent.RemoveListener(() => SetCurAmmoTmp(weaponSlotidx));
        gun[weaponSlotidx].OnReloadEvent.RemoveListener(() => SetInventoryAmmoTmp(weaponSlotidx));
    }
    #endregion

    public void InitTmp(int weaponSlotidx)
    {
        StartCoroutine(DelayedInitTmp(weaponSlotidx));
    }

    /// <summary>
    /// Gun이 완전히 초기화 되지 않고 tmp를 초기화하는 현상이 발생
    /// 한 프레임 미뤄서 메서드 실행
    /// </summary>
    /// <param name="weaponSlotidx"></param>
    /// <returns></returns>
    private IEnumerator DelayedInitTmp(int weaponSlotidx)
    {
        yield return null; // 한 프레임 대기
        SetInventoryAmmoTmp(weaponSlotidx);
    }

    //현재 CurrentAmmo를 FireEvent마다 갱신
    public void SetCurAmmoTmp(int weaponSlotidx)
    {
        if (gun[weaponSlotidx] == null) return;
        curBulletTmp.text = gun[weaponSlotidx].currentAmmo.ToString();
    }

    public void SetInventoryAmmoTmp(int weaponSlotidx)
    {
        if (gun[weaponSlotidx] == null) return;
        SetCurAmmoTmp(weaponSlotidx);
        inventoryBulletTmp.text = player.playerUI.inventoryUI.GetTotalAmmoFromInventory(gun[weaponSlotidx].gunData.bulletType).ToString();
    }
    //선택된 Slot UI반영
    public void ActiveWeaponSlot(int weaponSlotIdx)
    {
        switch (weaponSlotIdx)
        {
            case 0:
                toggleWeaponSlot1.SetActive(true);
                weaponTmp1.text = gun[weaponSlotIdx].gunData.gunName;
                _activeWeaponSlotIdx = weaponSlotIdx;
                break;
            case 1:
                toggleWeaponSlot2.SetActive(true);
                weaponTmp2.text = gun[weaponSlotIdx].gunData.gunName;
                _activeWeaponSlotIdx = weaponSlotIdx;
                break;
            default: return;
        }
    }

    public void DeActiveWeaponSlot(int weaopnSlotIdx)
    {
        switch (weaopnSlotIdx)
        {
            case 0:
                toggleWeaponSlot1.SetActive(false);
                break;
            case 1:
                toggleWeaponSlot2.SetActive(false);
                break;
            default: return;
        }
    }

    
    //버리거나 Change가 발생했을때 UI반영
    public void ClearSlot(int weaponSlotIdx)
    {
        if (weaponSlotIdx == 0)
        {
            toggleWeaponSlot1.SetActive(false);
            weaponTmp1.text = null;
        }
        else if (weaponSlotIdx == 1)
        {
            toggleWeaponSlot2.SetActive(false);
            weaponTmp2.text = null;
        }

        //슬록에 해당하는 UI만 초기화하도록 수정
        if (item[weaponSlotIdx] != null)
        {
            weaponIcon.sprite = item[weaponSlotIdx].itemData.icon;
            bulletIcon.sprite = item[weaponSlotIdx].itemData.bulletIcon;
        }
        else
        {
            weaponIcon.sprite = null;
            bulletIcon.sprite = null;
        }

        Color color = Color.white;
        color.a = (item[weaponSlotIdx] != null) ? 1f : 0f;
        weaponIcon.color = color;
        bulletIcon.color = color;

        //현재 슬롯의 탄약 정보만 초기화
        if (gun[weaponSlotIdx] != null)
        {
            curBulletTmp.text = gun[weaponSlotIdx].currentAmmo.ToString();
            inventoryBulletTmp.text = player.playerUI.inventoryUI.GetTotalAmmoFromInventory(gun[weaponSlotIdx].gunData.bulletType).ToString();
        }
        else
        {
            curBulletTmp.text = null;
            inventoryBulletTmp.text = null;
        }

        //아이템과 총 정보 비우기
        item[weaponSlotIdx] = null;
        gun[weaponSlotIdx] = null;
    }

    /// <summary>
    /// 외부에서 해당 메서드를 부른다
    /// Clear를 불러 UI를 반영
    /// </summary>
    /// <param name="weaponSlotIdx"></param>
    public void Clear(int weaponSlotIdx)
    {
        //이벤트 연결 해제
        DisConnectGunEvent(weaponSlotIdx);

        //현재 슬롯 비우기
        ClearSlot(weaponSlotIdx);

        //슬롯이 하나 남아 있을 경우 그 슬롯을 활성화
        int remainingSlotIdx = weaponSlotIdx == 0 ? 1 : 0;

        if (item[remainingSlotIdx] != null && gun[remainingSlotIdx] != null)
        {
            //UI 갱신
            SetImage(remainingSlotIdx);
            SetCurAmmoTmp(remainingSlotIdx);
            SetInventoryAmmoTmp(remainingSlotIdx);

            if (remainingSlotIdx == 0)
            {
                weaponTmp1.text = gun[remainingSlotIdx].gunData.gunName;
                toggleWeaponSlot1.SetActive(true);
                toggleWeaponSlot2.SetActive(false);
            }
            else
            {
                weaponTmp2.text = gun[remainingSlotIdx].gunData.gunName;
                toggleWeaponSlot2.SetActive(true);
                toggleWeaponSlot1.SetActive(false);
            }

            ConnectGunEvent(remainingSlotIdx);
        }
        else
        {
            //두 슬롯이 모두 비어 있으면 UI 초기화
            ClearSlot(remainingSlotIdx);
        }
    }
}

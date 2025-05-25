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
        //�������� �����Ҷ����� �ش� ���Կ� �´� ������ Bullet�� UI�� �ݿ�
        player.playerUI.inventoryUI.OnAcquireItem.AddListener(() => SetInventoryAmmoTmp(_activeWeaponSlotIdx));
    }
    //���⸦ �����ҋ� ȣ��Ǵ� �޼��� Wrapping ��
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
    //Sprite ����
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
    /// Gun�� ������ �ʱ�ȭ ���� �ʰ� tmp�� �ʱ�ȭ�ϴ� ������ �߻�
    /// �� ������ �̷Ｍ �޼��� ����
    /// </summary>
    /// <param name="weaponSlotidx"></param>
    /// <returns></returns>
    private IEnumerator DelayedInitTmp(int weaponSlotidx)
    {
        yield return null; // �� ������ ���
        SetInventoryAmmoTmp(weaponSlotidx);
    }

    //���� CurrentAmmo�� FireEvent���� ����
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
    //���õ� Slot UI�ݿ�
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

    
    //�����ų� Change�� �߻������� UI�ݿ�
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

        //���Ͽ� �ش��ϴ� UI�� �ʱ�ȭ�ϵ��� ����
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

        //���� ������ ź�� ������ �ʱ�ȭ
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

        //�����۰� �� ���� ����
        item[weaponSlotIdx] = null;
        gun[weaponSlotIdx] = null;
    }

    /// <summary>
    /// �ܺο��� �ش� �޼��带 �θ���
    /// Clear�� �ҷ� UI�� �ݿ�
    /// </summary>
    /// <param name="weaponSlotIdx"></param>
    public void Clear(int weaponSlotIdx)
    {
        //�̺�Ʈ ���� ����
        DisConnectGunEvent(weaponSlotIdx);

        //���� ���� ����
        ClearSlot(weaponSlotIdx);

        //������ �ϳ� ���� ���� ��� �� ������ Ȱ��ȭ
        int remainingSlotIdx = weaponSlotIdx == 0 ? 1 : 0;

        if (item[remainingSlotIdx] != null && gun[remainingSlotIdx] != null)
        {
            //UI ����
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
            //�� ������ ��� ��� ������ UI �ʱ�ȭ
            ClearSlot(remainingSlotIdx);
        }
    }
}

using UnityEngine;

public class PlayerEquipManager : MonoBehaviour
{
    [Header("Weapon Root")]
    public GameObject playerWeaponRoot;
    public GameObject[] weaponRootList;

    //���� �������� ���� idx
    public int curWeaponId;
    int grenadeSlotId = 3;
    private GunController _gunController;

    private Player _player;

    /// <summary>
    /// 1,2,3�� ���� ���⸦ �ٲ۴�
    /// 1,2 => �κ��丮 weaponSlot
    /// 3 = > �Ǽ�
    /// </summary>
    /// <param name="weaponId"></param>
    /// 

    private void Start()
    {
        _gunController = GetComponent<GunController>();
        _player = GetComponent<Player>();
        curWeaponId = 2;
        //ChangeWeaponSlot�̺�Ʈ�� ���� ����Ʈ ��ġ ���� �޼��� �߰�
        for (int i = 0; i < curWeaponId; i++)
        {
            _player.playerInventoryController.inventory.weaponSlots[i].OnChangeWeaponSlot.AddListener(ChangeWeaponSlotList);
        }
    }

    public void ToggleWeapon(int weaponId)
    {
        //curWeapon Id 0,1,2,
        //weaponID     0,1,2
        _player.fPSController._instantiatedWeapons[weaponId].gameObject.SetActive(true);
        if (curWeaponId == weaponId)
        {
            return;
        }
        //Fist�ΰ��
        if (weaponId == 2)
        {
            Debug.Log("�ָ�");
            weaponRootList[curWeaponId].SetActive(false);
            weaponRootList[weaponId].SetActive(true);
            curWeaponId = weaponId;
            _gunController.currentGun = null;
            _gunController.currentGrenade = null;
            //ũ�ν���� ����
            _player.playerUI.inGameUI.ui_crossHair.DeActiveCrossHairWeapon();
        }
        //weaponId 0 or 1
        else
        {
            //������ ��������� �ٲ����ʴ´�
            if (_player.playerInventoryController.inventory.weaponSlots[weaponId].item == null) return;
            weaponRootList[curWeaponId].SetActive(false);
            //ũ�ν���� �ѱ�
            _player.playerUI.inGameUI.ui_crossHair.ActiveCrossHairWeapon();
            _player.playerUI.inventoryUI.weaponSlots[weaponId].SetInGameWeaponUI();
            ActiveWeapon(weaponId);
        }

    }

    /// <summary>
    /// ����ź�� �����ϴ� �޼���
    /// �κ��丮�� ���� ����ź�� �����ϸ� ������ ����
    /// �ڽĿ�����Ʈ�� �����ϸ�(�̹� ����ź�� �����س������¸� �ش� ����ź�� ������ ������ ����)
    /// </summary>
    public void EquipGrenade()
    {
        Item grenade = _player.playerInventoryController.inventory.GetGrenadeFromInventory();

        //�̹� ����ź�� �����س��� ���
        if (HasChild(weaponRootList[grenadeSlotId].transform))
        {
            Debug.Log("�̹� ����ź�� �����Ǿ�����");
            ActiveGrenadeWeaponRoot();
            return;
        }
        //����ź�� ��������������
        if (grenade == null)
        {
            Debug.Log("����ź�� ������������");
            return;
        }



        GameObject go = Instantiate(grenade.itemData.equipPrefab, weaponRootList[grenadeSlotId].transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        //TODO : �����丵�Ҷ� ���̶� ����ź�̶� �Դٰ����Ҷ� GunController�� ������Ʈ �ϴ� ��� �����غ���
        _player.gunController.currentGrenade = go.GetComponent<Grenade>();

        ActiveGrenadeWeaponRoot();
    }
    /// <summary>
    /// �ڽ� ������Ʈ�� �����ϴ��� üũ
    /// </summary>
    /// <param name="slotIdx"></param>
    public bool HasChild(Transform parent)
    {
        return parent.childCount > 0;
    }

    /// <summary>
    /// ����ź ������ Active�ϴ� �޼���
    /// </summary>
    /// <param name="slotIdx"></param>
    public void ActiveGrenadeWeaponRoot()
    {
        weaponRootList[curWeaponId].SetActive(false);
        weaponRootList[grenadeSlotId].SetActive(true);
        curWeaponId = grenadeSlotId;

        _gunController.currentGun = null;
        _player.playerUI.inGameUI.ui_crossHair.DeActiveCrossHairWeapon();
    }

    //���⸦ ȹ���ϴ� �̺�Ʈ�� �߻��ϸ� �ش� Root�� �����ϱ�
    //Slot�Ű������� ���� ���޹���
    //�κ��丮 Init()�ɶ� ������
    public void AttachWeaponOnRoot(int slotIdx)
    {
        if (_player.playerInventoryController.inventory.weaponSlots[slotIdx].item != null)
        {
            GameObject go_prefab = _player.playerInventoryController.inventory.weaponSlots[slotIdx].
                item.itemData.equipPrefab;
            GameObject go = _player.fPSController.InitWeapon(go_prefab, slotIdx);

            _gunController.currentGun = go.GetComponent<Gun>();
            _player.playerInventoryController.inventory.weaponSlots[slotIdx].gun = go.GetComponent<Gun>();
            _player.playerInventoryController.inventory.weaponSlots[slotIdx].SetInGameWeaponUI();
            /*GameObject go = Instantiate(_player.playerInventoryController.inventory.weaponSlots[slotIdx].
                item.itemData.equipPrefab,
                playerWeaponRoot.transform);

            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            _player.playerInventoryController.inventory.weaponSlots[slotIdx].gun = go.GetComponent<Gun>();
            _player.playerInventoryController.inventory.weaponSlots[slotIdx].SetInGameWeaponUI();
            _gunController.currentGun = go.GetComponent<Gun>();*/
        }

        //ToggleWeapon(slotIdx);
    }



    /// <summary>
    /// Toggle�ÿ� �� ������Ʈ Ȱ��ȭ ��Ŵ
    /// </summary>
    /// <param name="weaponId"></param>
    public void ActiveWeapon(int weaponId)
    {
        weaponRootList[weaponId].SetActive(true);
        curWeaponId = weaponId;
        UpdateCurrentGun(weaponId);
        _gunController.currentGrenade = null;
    }

    /// <summary>
    /// Toggle�ÿ� ���� ���� ������Ʈ���־� ��Ʈ�ѷ����� �����ؾ���
    /// </summary>
    /// <param name="weaponId"></param>
    public void UpdateCurrentGun(int weaponId)
    {
        if (weaponRootList[weaponId].gameObject.transform.GetChild(0).TryGetComponent<Gun>(out Gun gun))
        {
            _gunController.currentGun = gun;
        }
    }

    /// <summary>
    /// ���� Ȱ��ȭ �Ǿ��ִ� ������Ʈ�� idx�� ��ȯ
    /// </summary>
    /// <param name="weaponId"></param>
    /// 
    public int GetCurrentWeaponRootIdx()
    {
        for (int i = 0; i < weaponRootList.Length; i++)
        {
            if (weaponRootList[i].gameObject.activeSelf)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// ����Ʈ ���Ը� �ٲ������ν� ������ �ѱⰡ �ٲ�µ��� ȿ���� ��
    /// </summary>
    private void ChangeWeaponSlotList(int slotIdx)
    {
        if (weaponRootList.Length < 2)
        {
            Debug.Log("����!");
            return;
        }
        GameObject go = weaponRootList[0];
        weaponRootList[0] = weaponRootList[1];
        weaponRootList[1] = go;
        //���� ����ִ� ���� �ٸ� ���Կ� �ű�� ���
        if (curWeaponId != slotIdx)
        {
            curWeaponId = slotIdx;
        }
        //0������ ����ִµ� 1������ 0������ �ű�� ���
        //curWeaponId == slotIdx�� ������
        //TODO :�ϵ��ڵ� ��ġ��
        else if (curWeaponId == slotIdx)
        {
            if (curWeaponId == 0) curWeaponId = 1;
            else curWeaponId = 0;
        }
    }
    //curweponId == 1
    //0��¥���� 1������ �Ű�� �׷� slotIdx = 1�� ���ðž�
    //�׷� curWeaponId == 0���� ����������

    public void DestroyWeapon(int weaponId)
    { 
        Destroy(_player.fPSController._instantiatedWeapons[weaponId + 1].gameObject);
        //Destroy(weaponRootList[weaponId].gameObject.transform.GetChild(0).gameObject);
    }


}

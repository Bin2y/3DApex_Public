using UnityEngine;

public class PlayerEquipManager : MonoBehaviour
{
    [Header("Weapon Root")]
    public GameObject playerWeaponRoot;
    public GameObject[] weaponRootList;

    //현재 장착중인 무기 idx
    public int curWeaponId;
    int grenadeSlotId = 3;
    private GunController _gunController;

    private Player _player;

    /// <summary>
    /// 1,2,3에 따라서 무기를 바꾼다
    /// 1,2 => 인벤토리 weaponSlot
    /// 3 = > 맨손
    /// </summary>
    /// <param name="weaponId"></param>
    /// 

    private void Start()
    {
        _gunController = GetComponent<GunController>();
        _player = GetComponent<Player>();
        curWeaponId = 2;
        //ChangeWeaponSlot이벤트에 슬롯 리스트 위치 변경 메서드 추가
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
        //Fist인경우
        if (weaponId == 2)
        {
            Debug.Log("주먹");
            weaponRootList[curWeaponId].SetActive(false);
            weaponRootList[weaponId].SetActive(true);
            curWeaponId = weaponId;
            _gunController.currentGun = null;
            _gunController.currentGrenade = null;
            //크로스헤어 끄기
            _player.playerUI.inGameUI.ui_crossHair.DeActiveCrossHairWeapon();
        }
        //weaponId 0 or 1
        else
        {
            //슬롯이 비어있으면 바꾸지않는다
            if (_player.playerInventoryController.inventory.weaponSlots[weaponId].item == null) return;
            weaponRootList[curWeaponId].SetActive(false);
            //크로스헤어 켜기
            _player.playerUI.inGameUI.ui_crossHair.ActiveCrossHairWeapon();
            _player.playerUI.inventoryUI.weaponSlots[weaponId].SetInGameWeaponUI();
            ActiveWeapon(weaponId);
        }

    }

    /// <summary>
    /// 수류탄을 장착하는 메서드
    /// 인벤토리로 부터 수류탄이 존재하면 가져와 생성
    /// 자식오브젝트가 존재하면(이미 수류탄을 장착해놓은상태면 해당 수류탄을 꺼내는 로직을 실행)
    /// </summary>
    public void EquipGrenade()
    {
        Item grenade = _player.playerInventoryController.inventory.GetGrenadeFromInventory();

        //이미 수류탄을 장착해놓은 경우
        if (HasChild(weaponRootList[grenadeSlotId].transform))
        {
            Debug.Log("이미 수류탄이 장착되어있음");
            ActiveGrenadeWeaponRoot();
            return;
        }
        //수류탄이 존재하지않을때
        if (grenade == null)
        {
            Debug.Log("수류탄이 존재하지않음");
            return;
        }



        GameObject go = Instantiate(grenade.itemData.equipPrefab, weaponRootList[grenadeSlotId].transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        //TODO : 리팩토링할때 총이랑 수류탄이랑 왔다갔다할때 GunController에 업데이트 하는 방법 생각해보기
        _player.gunController.currentGrenade = go.GetComponent<Grenade>();

        ActiveGrenadeWeaponRoot();
    }
    /// <summary>
    /// 자식 오브젝트가 존재하는지 체크
    /// </summary>
    /// <param name="slotIdx"></param>
    public bool HasChild(Transform parent)
    {
        return parent.childCount > 0;
    }

    /// <summary>
    /// 수류탄 슬롯을 Active하는 메서드
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

    //무기를 획득하는 이벤트가 발생하면 해당 Root에 장착하기
    //Slot매개변수를 직접 전달받음
    //인벤토리 Init()될때 구독됨
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
    /// Toggle시에 총 오브젝트 활성화 시킴
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
    /// Toggle시에 현재 총을 업데이트해주어 컨트롤러에서 인지해야함
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
    /// 현재 활성화 되어있는 웨폰루트의 idx를 반환
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
    /// 리스트 슬롯만 바꿔줌으로써 실제로 총기가 바뀌는듯한 효과를 줌
    /// </summary>
    private void ChangeWeaponSlotList(int slotIdx)
    {
        if (weaponRootList.Length < 2)
        {
            Debug.Log("작음!");
            return;
        }
        GameObject go = weaponRootList[0];
        weaponRootList[0] = weaponRootList[1];
        weaponRootList[1] = go;
        //내가 들고있는 총을 다른 슬롯에 옮기는 경우
        if (curWeaponId != slotIdx)
        {
            curWeaponId = slotIdx;
        }
        //0번총을 들고있는데 1번총을 0번으로 옮기는 경우
        //curWeaponId == slotIdx가 같아짐
        //TODO :하드코딩 고치기
        else if (curWeaponId == slotIdx)
        {
            if (curWeaponId == 0) curWeaponId = 1;
            else curWeaponId = 0;
        }
    }
    //curweponId == 1
    //0번짜리를 1번으로 옮겼어 그럼 slotIdx = 1이 들어올거야
    //그럼 curWeaponId == 0으로 만들어줘야해

    public void DestroyWeapon(int weaponId)
    { 
        Destroy(_player.fPSController._instantiatedWeapons[weaponId + 1].gameObject);
        //Destroy(weaponRootList[weaponId].gameObject.transform.GetChild(0).gameObject);
    }


}

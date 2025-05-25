using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Current Gun")]
    [SerializeField]
    public Gun currentGun;
    public Grenade currentGrenade;

    private PlayerInputManager _input;
    private PlayerInventoryController _inventoryController;
    private PlayerController _playerController;
    private Player _player;
    public bool isFiring = false;

    public float interval = 0f;
    [SerializeField] private bool isReloading;



    private void Start()
    {
        Init();
    }
    private void Update()
    {
        if (_input.fire && !isFiring && !_inventoryController.isOpen && _player.playerEquipManager.curWeaponId != 3)
        {
            if (currentGun == null) return;
            if (!currentGun.isBulletAvailable) return;
            if (currentGun.gunData.isAutomatic)
            {
                if (currentGun.isBulletAvailable == false) { return; }
                isFiring = true;
                InvokeRepeating(nameof(FireGun), 0f, currentGun.gunData.shootingInterval);
            }
            else
            {
                if (currentGun.gunData.shootingInterval < interval)
                {
                    isFiring = true;
                    FireGun();
                    interval = 0f;
                }
            }
        }
        //TODO : 수정해야함 InputAction으로
        else if (Input.GetMouseButtonDown(0) && currentGrenade!= null && !_inventoryController.isOpen)//_input.fire && _player.playerEquipManager.curWeaponId == 3)
        {
            currentGrenade._isShowingTrajectory = true;
            currentGrenade._lineRenderer.enabled = true;
        }
        else if (Input.GetMouseButtonUp(0) && currentGrenade != null  && !_inventoryController.isOpen)
        {
            currentGrenade._isShowingTrajectory = false;
            currentGrenade._lineRenderer.enabled = false;
            ThrowGrenade();
        }
        if (!_input.fire && isFiring)
        {
            if (currentGun == null) return;
            isFiring = false;
            CancelInvoke(nameof(FireGun));
        }

        interval += Time.deltaTime;
    }
    private void Init()
    {
        _input = GetComponent<PlayerInputManager>();
        _inventoryController = GetComponent<PlayerInventoryController>();
        _playerController = GetComponent<PlayerController>();
        _player = GetComponent<Player>();
    }

    public void RemoveWeapon()
    {
        currentGun = null;
    }

    public void RemoveGreande()
    {
        currentGrenade = null;
    }

    private void ThrowGrenade()
    {
        if (currentGrenade == null) return;

        //복사본 생성
        GameObject grenadeClone = Instantiate(currentGrenade.gameObject);
        grenadeClone.transform.position = currentGrenade.transform.position;
        grenadeClone.transform.rotation = currentGrenade.transform.rotation;

        Grenade grenadeComponent = grenadeClone.GetComponent<Grenade>();

        _player.playerInventoryController.inventory.ConsumeGrenade(grenadeComponent.currentGrenadeType);
        grenadeComponent.Throw();
        
        

        // 폭발 타이머 시작
        StartCoroutine(grenadeComponent.Explosion());

        RemoveGreande();
    }
    private void FireGun()
    {
        if (currentGun == null) return;
        if (currentGun.TryFire()) { }
    }

    public bool ReloadWeapon()
    {
        int ammo = _inventoryController.inventory.GetBulletFromInventory(
            currentGun.gunData.bulletType,
            currentGun.currentAmmo,
            currentGun.gunData.maxBulletCnt);
        if (isReloading || currentGun.currentAmmo == currentGun.gunData.maxBulletCnt || isFiring || ammo == 0) 
            return false;
        StartCoroutine(ReloadCoroutine(ammo));
        return true;
    }
    private IEnumerator ReloadCoroutine(int ammo)
    {
        isReloading = true;
        yield return new WaitForSeconds(currentGun.gunData.reloadTime);

        //인벤토리에 GetBulletFromInventory 메서드를 실행해서 총알타입, 최대총알 갯수 정보를 넘김
        currentGun.currentAmmo += ammo;

        currentGun.CheckBullet();
        currentGun.OnReloadEvent?.Invoke();
        isReloading = false;
    }
}

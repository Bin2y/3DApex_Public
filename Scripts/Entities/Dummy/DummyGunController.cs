using UnityEngine;

public class DummyGunController : MonoBehaviour
{
    public DummyGun currentGun;

    public bool isFiring = false;

    public float interval = 0f;
    [SerializeField] private bool isReloading;

    public void Init(Player player)
    {
        currentGun.target = player.transform;
    }
    public void FireGun()
    {
        if (currentGun == null) return;
        currentGun.TryFire();
    }

    public bool ReloadWeapon()
    {
        int ammo = currentGun.gunData.maxBulletCnt;
        currentGun.currentAmmo = ammo;
        return true;
    }

}


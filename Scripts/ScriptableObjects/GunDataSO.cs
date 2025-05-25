using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "GunScriptable/CreateGunData")]
public class GunDataSO : ScriptableObject
{
    [Header("Name")]
    public string gunName;

    [Header("Bullet")]
    public Define.bulletType bulletType;
    public int maxBulletCnt;
    public float reloadTime;
    public int damage;
    public GameObject bullet;

    [Header("Recoil")]
    public float recoilAmount;

    [Header("Gun Feature")]
    [Tooltip("사격간격")]
    public float shootingInterval;
    //총이 단발형인지 연사형인지 체크
    public bool isAutomatic;
}


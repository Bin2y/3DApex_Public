using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Projectile/CreateBulletData")]
public class BulletDataSO : ScriptableObject
{
    [Header("Bullet")]
    [Tooltip("Speed of Bullet in m/s")]
    public float bulletSpeed;
    [Tooltip("Range of Bullet")]
    public float bulletRange;
    [Tooltip("Gravity of Bullet")]
    public float gravity;
    [Tooltip("Damage of Bullet")]
    public int damage;
    [Tooltip("Bullet Mark Object(Decal)")]
    public GameObject decalPrefab;

}

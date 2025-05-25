using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "ItemScriptable/CreateItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    /// <summary>
    /// Light, Heavy, Energy,
    /// </summary>
    [Header("Info")]
    public int itemID;
    public string itemName;
    public string description;
    public Define.ItemType type;
    public Sprite icon;
    public Sprite bulletIcon;
    public GameObject equipPrefab;
    public GameObject dropPrefab;
    [Header("Consume")]
    public bool isConsumable;
    public float consumeTime;

    [Header("Stacking")]
    public bool canStack;
    public int maxCount;

    [Header("Bullet")]
    public bool isBullet;
    public Define.bulletType bulletType;

    [Header("Heal")]
    public Define.HealType healType;

    [Header("Greande")]
    public Define.GrenadeType grenadeType;
}
//Gun BulletType이랑 비교해야함



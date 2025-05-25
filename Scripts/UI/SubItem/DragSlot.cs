using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance;
    public ISlot curSlot;

    [SerializeField]
    private Image imageItem;

    void Start()
    {
        instance = this;
    }

    public void DragSetImage(Image _itemImage)
    {
        imageItem.sprite = _itemImage.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }

    /// <summary>
    /// 버리는 존에 가면 아이템을 버림
    /// </summary>
    public void DiscardItem()
    {
        Transform playerTransform = GameManager.Instance.player.transform;
        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * 2f + playerTransform.up * 2f;
        GameObject go = Instantiate(curSlot.GetItemData().itemData.dropPrefab, spawnPosition, Quaternion.identity);
        //5f << 아이템이 날라가는 힘
        go.GetComponent<Rigidbody>().AddForce(playerTransform.forward * 5f, ForceMode.Impulse);
        go.GetComponent<ItemObject>().quantity = curSlot.GetItemData().quantity;
        curSlot.ClearSlot();
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_UsingItem : UI_SubItem
{
    [Header("ItemImage")]
    [SerializeField] public Image itemImage;

    [Header("ItemPrompt")]
    [SerializeField] public Image timerBar;
    [SerializeField] public TextMeshProUGUI timerTmp;
    [SerializeField] public TextMeshProUGUI itemNameTmp;


    public void SetUI(Item item)
    {
        itemImage.sprite = item.itemData.icon;
        itemNameTmp.text = item.itemData.itemName;
        gameObject.SetActive(true);
    }

    public void SetItemTImer(float progress, float timer)
    {
        timerBar.fillAmount = 1 - progress;
        timerTmp.text = timer.ToString("F1");
    }

    public void ResetTimer()
    {
        timerBar.fillAmount = 0f;
        gameObject.SetActive(false);
    }
}

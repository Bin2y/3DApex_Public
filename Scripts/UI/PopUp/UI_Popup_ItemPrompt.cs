using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_ItemPrompt : MonoBehaviour
{
    public DG_UI_FadeEffect dg_ui_fadeEffect;
    [Header("Head")]
    public TextMeshProUGUI changeInfo;
    public TextMeshProUGUI keyInfo;

    [Header("Body")]
    public Image itemImage;
    public TextMeshProUGUI roleTmp;
    public TextMeshProUGUI itemNameTmp;
    public TextMeshProUGUI itemDescriptionTmp;
    public TextMeshProUGUI typeTmp;

    public void Init()
    {
        dg_ui_fadeEffect = GetComponent<DG_UI_FadeEffect>();
        gameObject.SetActive(true);
    }

    public void SetPrompt(ItemDataSO itemData)
    {
        if (!gameObject.activeSelf) { gameObject.SetActive(true); }
        itemImage.sprite = itemData.icon;
        itemNameTmp.text = itemData.itemName;
        itemDescriptionTmp.text = itemData.description;
    }

}

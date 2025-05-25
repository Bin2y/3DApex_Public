using TMPro;
using UnityEngine;

public class UI_SubItem_GrenadeSlot : UI_SubItem
{
    public TextMeshProUGUI itemCntTMP;
    /// <summary>
    /// 아래 두 개는 각 슬롯에 미리 저장해놓는 데이터
    /// 슬롯이 하이라이트 됐을때 중앙에 표기
    /// </summary>
    [Header("Item Desc Data")]
    public string itemNameString;
    public string descString;
    public Define.GrenadeType grenadeType;

    public void UpdateUI(int cnt)
    {
        itemCntTMP.text = cnt.ToString();
    }
}
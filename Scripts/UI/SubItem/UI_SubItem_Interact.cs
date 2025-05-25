using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_Interact : MonoBehaviour
{
    //상호작용 시간만큼 Fill할 이미지
    [SerializeField] private Image circleImage;
    [SerializeField] private TextMeshProUGUI interactTmp;


    public void PopupInteractUI(float nowTime, float holdDuration)
    {
        circleImage.fillAmount = nowTime / holdDuration;
    }
}

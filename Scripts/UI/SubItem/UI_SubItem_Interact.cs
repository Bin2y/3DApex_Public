using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_Interact : MonoBehaviour
{
    //��ȣ�ۿ� �ð���ŭ Fill�� �̹���
    [SerializeField] private Image circleImage;
    [SerializeField] private TextMeshProUGUI interactTmp;


    public void PopupInteractUI(float nowTime, float holdDuration)
    {
        circleImage.fillAmount = nowTime / holdDuration;
    }
}

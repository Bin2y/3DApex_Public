using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_Abliity : MonoBehaviour
{
    [Header("Icon")]
    public Image tacticalSkillIcon;
    public Image ultimateSkillIcon;

    [Header("CoolDown Image")]
    //tactical
    public Image tsCoolDownImage;
    //ultimate
    public Image usCoolDownImage;

    [Header("CoolDown TMP")]
    public TextMeshProUGUI tsCoolDownTMP;
    public TextMeshProUGUI usCoolDownTMP;

    private Player player;


    private void OnDisable()
    {
        player.skill.OnTacticalCoolDownChanged -= UpdateTSUI;
        player.skill.OnUltimateCoolDownChanged -= UpdateUSUI;
    }
    public void Init(Player player)
    {
        this.player = player;
        SetAbilityIcon();
        //�̺�Ʈ�� UI���� ����
        player.skill.OnTacticalCoolDownChanged += UpdateTSUI;
        player.skill.OnUltimateCoolDownChanged += UpdateUSUI;
    }

    //Init�ÿ� ������ ����
    private void SetAbilityIcon()
    {
        Color color = Color.white;
        color.a = 255f;
        tacticalSkillIcon.sprite = player.skill.skillDataSO.taticalSkillIcon;
        ultimateSkillIcon.sprite = player.skill.skillDataSO.ultimateSkillIcon;
        tacticalSkillIcon.color = color;
        ultimateSkillIcon.color = color;
    }


    //UI������Ʈ
    private void UpdateTSUI(float newCoolDown)
    {
        tsCoolDownImage.fillAmount = newCoolDown / player.skill.skillDataSO.taticalSkillCoolDown;
        if (tsCoolDownImage.fillAmount <= 0) tsCoolDownTMP.enabled = false;
        tsCoolDownTMP.enabled = true;
        tsCoolDownTMP.text = Mathf.FloorToInt(newCoolDown).ToString();
    }
    private void UpdateUSUI(float newCoolDown)
    {
        usCoolDownImage.fillAmount = newCoolDown / player.skill.skillDataSO.ultimateSkillCoolDown;
        if (Mathf.Approximately(usCoolDownImage.fillAmount, 0))
        {
            usCoolDownTMP.enabled = false;
        }
        usCoolDownTMP.enabled = true;
        usCoolDownTMP.text = (100 - Mathf.FloorToInt(((newCoolDown / player.skill.skillDataSO.ultimateSkillCoolDown) * 100))).ToString() + "%";
    }
}

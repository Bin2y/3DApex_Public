using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_HPShieldBar : UI_SubItem
{
    private HealthSystem healthSystem;
    [Header("HPShieldBar GameObject")]
    [SerializeField] private List<GameObject> shieldBarList;


    [Header("HPShieldBar Image")]
    [SerializeField] private Image healthImage;
    [SerializeField] private Image ShieldImage0;
    [SerializeField] private Image ShieldImage1;
    [SerializeField] private Image ShieldImage2;
    [SerializeField] private Image ShieldImage3;
    [SerializeField] private Image[] shieldImageArray;



    [Header("Core, Helmet")]
    [SerializeField] private GameObject core;
    [SerializeField] private GameObject helmetImage;

    public virtual void Init(HealthSystem targetHealthSystem)
    {
        healthSystem = targetHealthSystem;
        if(healthSystem == null)
        {
            Debug.LogError("HealthSystem이 미할당");
            return;
        }
        UpdateShieldSegments();
        UpdateBodyArmorIcon();
        healthSystem.OnHealthShieldChanaged.AddListener(UpdateHealthShield);
    }



    protected void UpdateHealthShield()
    {
        healthImage.fillAmount = (float)healthSystem.GetCurHealth() / healthSystem.GetMaxHealth();

        int shield = healthSystem.GetCurShield();
        int shieldSegmentCnt = 4;
        for (int i = 0; i < shieldSegmentCnt; i++)
        {
            int shieldSegmentMin = i * healthSystem.GetShieldAmountPerSegment();
            int shieldSegmentMax = (i + 1) * healthSystem.GetShieldAmountPerSegment();

            if (shield <= shieldSegmentMin)
            {
                //쉴드량이 현재 쉴드 최소값보다 작은경우
                shieldImageArray[i].fillAmount = 0f;
            }
            else
            {
                if (shield >= shieldSegmentMax)
                {
                    //쉴드량이 맥스박보다 많을 경우
                    shieldImageArray[i].fillAmount = 1f;
                }
                else
                {
                    // 최소와 맥스 중간 사이
                    float fillAmount = (float)(shield - shieldSegmentMin) / healthSystem.GetShieldAmountPerSegment();
                    shieldImageArray[i].fillAmount = fillAmount;
                }
            }
        }
    }

    public void UpdateShieldSegments()
    {
        Define.BodyArmor bodyArmor = healthSystem.armorSystem.GetEquippedBodyArmor();

        Color bodyArmorColor = Color.white;
        for (int i = 0; i < shieldBarList.Count; i++)
        {
            shieldBarList[i].SetActive(false);
        }
        switch (bodyArmor)
        {
            default:
            case Define.BodyArmor.None:
                break;
            case Define.BodyArmor.Tier_1:
                shieldBarList[0].SetActive(true);
                shieldBarList[1].SetActive(true);
                bodyArmorColor = healthSystem.armorSystem.TIER_1_COLOR;
                break;
            case Define.BodyArmor.Tier_2:
                shieldBarList[0].SetActive(true);
                shieldBarList[1].SetActive(true);
                shieldBarList[2].SetActive(true);
                bodyArmorColor = healthSystem.armorSystem.TIER_2_COLOR;
                break;
            case Define.BodyArmor.Tier_3:
                shieldBarList[0].SetActive(true);
                shieldBarList[1].SetActive(true);
                shieldBarList[2].SetActive(true);
                shieldBarList[3].SetActive(true);
                bodyArmorColor = healthSystem.armorSystem.TIER_3_COLOR;
                break;
        }
        ShieldImage0.color = bodyArmorColor;
        ShieldImage1.color = bodyArmorColor;
        ShieldImage2.color = bodyArmorColor;
        ShieldImage3.color = bodyArmorColor;
    }

    public void UpdateBodyArmorIcon()
    {
        Define.BodyArmor bodyArmor = healthSystem.armorSystem.GetEquippedBodyArmor();
        if(core!=null)
            core.SetActive(false);

        switch (bodyArmor)
        {
            default:
            case Define.BodyArmor.None:
                break;
            case Define.BodyArmor.Tier_1:
                if (core == null) return;
                core.SetActive(true);
                core.transform.Find("BackGround").GetComponent<Image>().color = healthSystem.armorSystem.TIER_1_COLOR;
                break;
            case Define.BodyArmor.Tier_2:
                if (core == null) return;
                core.SetActive(true);
                core.transform.Find("BackGround").GetComponent<Image>().color = healthSystem.armorSystem.TIER_2_COLOR;
                break;
            case Define.BodyArmor.Tier_3:
                if (core == null) return;
                core.SetActive(true);
                core.transform.Find("BackGround").GetComponent<Image>().color = healthSystem.armorSystem.TIER_3_COLOR;
                break;
        }
    }
}

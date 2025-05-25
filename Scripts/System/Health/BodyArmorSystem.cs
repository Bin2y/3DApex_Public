using UnityEngine;

public class BodyArmorSystem
{
    [Header("BodyArmor")]
    public Color TIER_1_COLOR = Color.white;
    public Color TIER_2_COLOR = Color.blue;
    public Color TIER_3_COLOR = UnityExtension.HexColor("8A2BE2");

    [Header("Damage that need to Upgrade")]
    //아머 쉴드 업그레이드에 필요한 데미지량
    private int TIER_1_TO_TIER_2 = 750;
    private int TIER_2_TO_TIER_3 = 1500;

    //현재 아머를 나타냄
    public Define.BodyArmor bodyArmor;
    public int damageAccumulated;
    private Player player;
    
    public BodyArmorSystem()
    {
        //TODO : 랜덤 아머 더미에게 주기
        bodyArmor = Define.BodyArmor.Tier_1;
    }
    public BodyArmorSystem(Player player)
    {
        bodyArmor = Define.BodyArmor.Tier_1;
        damageAccumulated = 0;
        this.player = player;
    }

    //현재 아머를 Get
    public Define.BodyArmor GetEquippedBodyArmor()
    {
        return bodyArmor;
    }

    //새로운 아머를 Set
    public void SetEquippedBodyArmo(Define.BodyArmor bodyArmor)
    {
        this.bodyArmor = bodyArmor;
    }

    
}

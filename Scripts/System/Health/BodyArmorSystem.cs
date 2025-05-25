using UnityEngine;

public class BodyArmorSystem
{
    [Header("BodyArmor")]
    public Color TIER_1_COLOR = Color.white;
    public Color TIER_2_COLOR = Color.blue;
    public Color TIER_3_COLOR = UnityExtension.HexColor("8A2BE2");

    [Header("Damage that need to Upgrade")]
    //�Ƹ� ���� ���׷��̵忡 �ʿ��� ��������
    private int TIER_1_TO_TIER_2 = 750;
    private int TIER_2_TO_TIER_3 = 1500;

    //���� �ƸӸ� ��Ÿ��
    public Define.BodyArmor bodyArmor;
    public int damageAccumulated;
    private Player player;
    
    public BodyArmorSystem()
    {
        //TODO : ���� �Ƹ� ���̿��� �ֱ�
        bodyArmor = Define.BodyArmor.Tier_1;
    }
    public BodyArmorSystem(Player player)
    {
        bodyArmor = Define.BodyArmor.Tier_1;
        damageAccumulated = 0;
        this.player = player;
    }

    //���� �ƸӸ� Get
    public Define.BodyArmor GetEquippedBodyArmor()
    {
        return bodyArmor;
    }

    //���ο� �ƸӸ� Set
    public void SetEquippedBodyArmo(Define.BodyArmor bodyArmor)
    {
        this.bodyArmor = bodyArmor;
    }

    
}

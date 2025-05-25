using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/CreateSkillData")]
public class SkillDataSO : ScriptableObject
{
    [Header("Tactical skill")]
    public string taticalSkillName;
    public float taticalSkillCoolDown;
    public Sprite taticalSkillIcon;
    //���ӽð��� �ִ� �ñر⳪ ��ų���� ���ӽð� ������ ��ٿ� ����
    public bool hasTacticalDuration = false;
    [Header("Ultimate")]
    public string ultimateSkilllName;
    public float ultimateSkillCoolDown;
    public Sprite ultimateSkillIcon;
    //���ӽð��� �ִ� �ñر⳪ ��ų���� ���ӽð� ������ ��ٿ� ����
    public bool hasUltimateDuration = false;
}

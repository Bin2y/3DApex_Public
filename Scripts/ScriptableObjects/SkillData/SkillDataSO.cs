using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/CreateSkillData")]
public class SkillDataSO : ScriptableObject
{
    [Header("Tactical skill")]
    public string taticalSkillName;
    public float taticalSkillCoolDown;
    public Sprite taticalSkillIcon;
    //지속시간이 있는 궁극기나 스킬들은 지속시간 종료후 쿨다운 시작
    public bool hasTacticalDuration = false;
    [Header("Ultimate")]
    public string ultimateSkilllName;
    public float ultimateSkillCoolDown;
    public Sprite ultimateSkillIcon;
    //지속시간이 있는 궁극기나 스킬들은 지속시간 종료후 쿨다운 시작
    public bool hasUltimateDuration = false;
}

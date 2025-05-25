using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] public SkillDataSO skillDataSO;

    //모두 구현해야함 사용자는 ActiveTactiaclSkill과 ActiveUltimateSkill만 불러서 사용
    public abstract void ActiveTacticalSkill();
    public abstract void ActiveUltimateSkill();
    //숫자 바뀔때 발생하는 Event
    public event Action<float> OnTacticalCoolDownChanged;
    public event Action<float> OnUltimateCoolDownChanged;

    [Header("스킬 쿨다운 계산")]
    private float _tacticalSkillCooldown;
    public float tacticalSkillCooldown
    {
        get => _tacticalSkillCooldown;
        set
        {
            if (Mathf.Approximately(_tacticalSkillCooldown, value)) return;

            _tacticalSkillCooldown = value;
            OnTacticalCoolDownChanged?.Invoke(value);
        }
    }
    public float _ultimateSkillCooldown;
    public float ultimateSkillCooldown
    {
        get => _ultimateSkillCooldown;
        set
        {
            if (Mathf.Approximately(_ultimateSkillCooldown, value)) return;

            _ultimateSkillCooldown = value;
            OnUltimateCoolDownChanged?.Invoke(value);
        }
    }


    public bool IsTacticalSkillCoolDown()
    {
        if (_tacticalSkillCooldown >= 0) return false;
        return true;
    }
    public bool IsUltimateSkillCoolDown()
    {
        if (_ultimateSkillCooldown >= 0) return true;
        return false;
    }

    private void Start()
    {
        //스킬쿨다운 초기화
        _tacticalSkillCooldown = -1f;
        _ultimateSkillCooldown = -1f;
    }
}

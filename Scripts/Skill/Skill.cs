using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] public SkillDataSO skillDataSO;

    //��� �����ؾ��� ����ڴ� ActiveTactiaclSkill�� ActiveUltimateSkill�� �ҷ��� ���
    public abstract void ActiveTacticalSkill();
    public abstract void ActiveUltimateSkill();
    //���� �ٲ� �߻��ϴ� Event
    public event Action<float> OnTacticalCoolDownChanged;
    public event Action<float> OnUltimateCoolDownChanged;

    [Header("��ų ��ٿ� ���")]
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
        //��ų��ٿ� �ʱ�ȭ
        _tacticalSkillCooldown = -1f;
        _ultimateSkillCooldown = -1f;
    }
}

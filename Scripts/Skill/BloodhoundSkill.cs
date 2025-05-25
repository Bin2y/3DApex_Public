using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodhoundSkill : Skill
{
    [Header("Tactical Skill Setting")]
    public float scanRadius = 50f;
    public float coneAngle = 125f;
    public float scanDuration = 2f;
    public LayerMask detectLayer;
    [Header("Ultimate Skill Setting")]
    public float ultimateDuration;
    public float movementSpeedBoost = 1.3f;

    [SerializeField] private bool isUltimateActive = false;
    private HighLight highLight;



    private void OnEnable()
    {
        highLight = GetComponent<HighLight>();
    }

    //전술 스킬 발동
    public override void ActiveTacticalSkill()
    {
        if (!IsTacticalSkillCoolDown())
        {
            return;
        };
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, scanRadius, detectLayer);
        List<GameObject> detectedDummies = new List<GameObject>();
        foreach (var hitCollider in hitColliders)
        {
            Vector3 direction = hitCollider.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);

            if (angle <= coneAngle / 2)
            {
                //오브젝트 하이라이트하기
                detectedDummies.Add(hitCollider.gameObject);
            }
        }
        if (detectedDummies.Count > 0)
        {
            highLight.HightlightCharacter(detectedDummies);
        }
        //스킬 쿨다운시작
        tacticalSkillCooldown = skillDataSO.taticalSkillCoolDown;
        StartCoroutine(ReduceTsCoolDown());
        //일정 시간 후 하이라이트 해제
        StartCoroutine(ResetHighlight());
    }

    private IEnumerator ReduceTsCoolDown()
    {
        while (tacticalSkillCooldown > 0)
        {
            tacticalSkillCooldown -= Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator ResetHighlight()
    {
        yield return new WaitForSeconds(scanDuration);
        Debug.Log("스캔 종료");
    }

    //궁극기 발동
    public override void ActiveUltimateSkill()
    {
        if (isUltimateActive || IsUltimateSkillCoolDown()) return;
        isUltimateActive = true;

        //쿨다운 시작 0초부터 증가
        ultimateSkillCooldown = skillDataSO.ultimateSkillCoolDown;
        Debug.Log(ultimateSkillCooldown);
        StartCoroutine(IncreaseUSCoolDown());
        //TODO 이동 속도 증가 추가
        StartCoroutine(UltimateDuration());
    }
    private IEnumerator IncreaseUSCoolDown()
    {
        while (ultimateSkillCooldown > 0)
        {
            ultimateSkillCooldown -= Time.deltaTime;
            yield return null;
        }
        isUltimateActive = false;
    }
    private IEnumerator UltimateDuration()
    {
        float timeLeft = ultimateDuration;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        //이동 속도 정상복구
        isUltimateActive = false;
    }
}

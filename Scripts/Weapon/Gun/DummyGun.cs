using UnityEngine;

public class DummyGun : Gun
{
    public Transform target; //플레이어

    protected override void Init()
    {
        base.Init();
        //mainCamera는 사용하지 않음
    }

    public override void Fire()
    {
        if (target == null)
        {
            Debug.LogWarning("DummyGun: No target set for firing.");
            return;
        }

        //타겟이 너무 가까우면 최소 거리 확보
        Vector3 targetPoint = target.position;
        float minDistance = 5.0f;
        if (Vector3.Distance(fireTransform.position, targetPoint) < minDistance)
        {
            targetPoint = fireTransform.position + (target.position - fireTransform.position).normalized * minDistance;
        }

        //탄 퍼짐 & 반동 계산
        Vector3 shootDir = (targetPoint - fireTransform.position).normalized;
        //shootDir = shootDir;
        Shoot(shootDir);
    }

    private void Shoot(Vector3 shootDir)
    {
        currentAmmo--;

        GameObject bullet = PoolManager.Instance.GetGameObject("Bullet");
        bullet.SetActive(true);
        bullet.transform.position = fireTransform.position;
        bullet.transform.rotation = Quaternion.LookRotation(shootDir);
    }
}
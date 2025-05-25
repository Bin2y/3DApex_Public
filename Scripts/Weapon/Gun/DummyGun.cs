using UnityEngine;

public class DummyGun : Gun
{
    public Transform target; //�÷��̾�

    protected override void Init()
    {
        base.Init();
        //mainCamera�� ������� ����
    }

    public override void Fire()
    {
        if (target == null)
        {
            Debug.LogWarning("DummyGun: No target set for firing.");
            return;
        }

        //Ÿ���� �ʹ� ������ �ּ� �Ÿ� Ȯ��
        Vector3 targetPoint = target.position;
        float minDistance = 5.0f;
        if (Vector3.Distance(fireTransform.position, targetPoint) < minDistance)
        {
            targetPoint = fireTransform.position + (target.position - fireTransform.position).normalized * minDistance;
        }

        //ź ���� & �ݵ� ���
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
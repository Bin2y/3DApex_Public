using UnityEngine;

public class R301 : Gun
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        base.Init();

    }

    public override void Fire()
    {
        ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
            float minDistance = 5.0f;
            if (Vector3.Distance(fireTransform.position, targetPoint) < minDistance)
            {
                targetPoint = ray.GetPoint(minDistance);
            }
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }
        Vector3 shootDir = (targetPoint - fireTransform.position).normalized;
        Shoot(shootDir);
    }

    protected override void Shoot(Vector3 shootDir)
    {
        base.Shoot(shootDir);
        //Instantiate(gunData.bullet, fireTransform.position, Quaternion.LookRotation(shootDir));
    }
}

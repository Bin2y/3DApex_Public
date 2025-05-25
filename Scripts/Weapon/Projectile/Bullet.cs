using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Data")]
    public BulletDataSO bulletData;
    

    private Rigidbody _rb;
    private Collider _collider;
    private PoolAble _poolAble;

    private WaitForSeconds bulletSurviveTime = new WaitForSeconds(3f);

    private Vector3 previousPosition;
    private bool hasStarted = false;


    public DamageMessage _damageMessage;
    private GameObject _shooter;
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _poolAble = GetComponent<PoolAble>();
    }
    protected virtual void Start()
    {
        SetDamageMessage();
    }

    private void OnEnable()
    {
        previousPosition = transform.position;
    }

    protected virtual void Update()
    {
        CheckOutofRange();
    }
    protected virtual void FixedUpdate()
    {
        BulletMove();
        if(hasStarted)
        {
            CheckBulletCollision();
        }
        else
        {
            hasStarted = true;
        }

        previousPosition = transform.position;
    }
    private void CheckBulletCollision()
    {
        Vector3 direction = transform.position - previousPosition;
        float distance = direction.magnitude;
        

        if (distance > 0)
        {
            //SphereCast를 사용하여 작은 구체로 충돌 체크 (더 정확함)
            if (Physics.SphereCast(previousPosition, 0.0005f, direction.normalized, out RaycastHit hit, distance))
            {
                HandleHit(hit);
            }
        }

    }


    //실제 충돌 로직을 처리하는 메서드
    private void HandleHit(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent<IDamageable>(out IDamageable IDamage))
        {
            IDamage.ApplyDamage(_damageMessage);
        }
        else
        {
            if (bulletData.decalPrefab != null)
            {
                //Collider에서 충돌 지점을 직접 가져오기
                Vector3 hitPoint = transform.position;
                Vector3 hitNormal = -transform.forward;//총알 진행 방향 반대로 Decal 정렬

                GameObject decal = Instantiate(bulletData.decalPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
                decal.transform.SetParent(hit.transform);//충돌한 오브젝트에 고정
            }
        }
        _poolAble.ReleaseObject();
    }

    public void SetBulletData(BulletDataSO bulletData)
    {
        this.bulletData = bulletData;
    }

    public void SetShooter(GameObject go)
    {
        _shooter = go;
    }

    /// <summary>
    /// Send Damage Message to hit Person
    /// </summary>
    protected virtual void SetDamageMessage()
    {
        _damageMessage.damager = _shooter;
        _damageMessage.amount = bulletData.damage;
        _damageMessage.hitPoint = Vector3.zero;
        _damageMessage.hitNormal = Vector3.zero;
        _damageMessage.hitPoint = Vector3.zero;
    }

    private void BulletMove()
    {
        _rb.velocity = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z) * bulletData.bulletSpeed * Time.fixedDeltaTime;
    }
    private void CheckOutofRange()
    {
        StartCoroutine(CheckOutofTime());
    }


    private IEnumerator CheckOutofTime()
    {
        yield return bulletSurviveTime;
        _poolAble.ReleaseObject();
    }
}

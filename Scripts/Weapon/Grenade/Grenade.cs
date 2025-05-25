using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Grenade Physics")]
    public Rigidbody rb;
    public float pulse;
    //수류탄 종류별로 터지는 타이밍이 다름
    public float explosionTime;
    //터지는 범위
    public float explosionRadius;
    private DamageMessage damageMessage;
    public DamageMessage collideDamageMessage;
    [Header("Effect & Sound")]
    public GameObject meshObj;
    public GameObject effectObj;

    [Header("LineRender")]
    public LineRenderer _lineRenderer;
    public Camera _mainCamera;
    public bool _isShowingTrajectory = false;

    [Header("Value")]
    public Vector3 position;
    public Vector3 forward;
    public Vector3 startVelocity;
    public Vector3 startPosition;
    public float throwPower;
    public float damageAmount;

    public Coroutine throwCorutine;
    public Define.GrenadeType currentGrenadeType;
    private void Start()
    {
        damageMessage = new DamageMessage();
        damageMessage.damager = this.gameObject;
        damageMessage.amount = 100;

        //LineRenderer
        _mainCamera = Camera.main;
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isShowingTrajectory)
        {
            ShowTrajectory();
        }
    }

    public void Throw()
    {
        position = transform.position;
        forward = _mainCamera.transform.forward;
        startVelocity = throwPower * forward;
        startPosition = position - transform.right * 2 - transform.up;
        startPosition.y -= 2f;

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        Rigidbody goRb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(startVelocity, ForceMode.Impulse);

    }

    private void ShowTrajectory()
    {
        int linePoint = 1;
        float timePoint = 0.1f;

        _lineRenderer.positionCount = Mathf.CeilToInt(linePoint / timePoint) + 1;

        forward = _mainCamera.transform.forward;
        startVelocity = throwPower * forward;
        startPosition = transform.position - transform.right * 2 - transform.up;
        startPosition.y -= 2f;

        _lineRenderer.SetPosition(0, startPosition);

        for (int i = 1; i < _lineRenderer.positionCount; i++)
        {
            float time = i * timePoint;

            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            _lineRenderer.SetPosition(i, point);
        }
    }

    public IEnumerator Explosion()
    {
        yield return new WaitForSeconds(explosionTime);
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        meshObj.SetActive(false);
        effectObj.SetActive(true);

        HitGreande();
    }

    protected void HitGreande()
    {
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.up, 0f);

        foreach (RaycastHit hit in rayHits)
        {
            if (hit.transform.gameObject.TryGetComponent<IDamageable>(out IDamageable component))
            {
                component.ApplyDamage(damageMessage);
            }
        }

        Destroy(gameObject, explosionTime + 2f);
    }
}



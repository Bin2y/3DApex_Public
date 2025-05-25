using UnityEngine;
using UnityEngine.AI;


public enum DummyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Reload,
    Dead
}

public class DummyController : MonoBehaviour
{
    public enum DummyState { Idle, Patrol, Chase, Attack, Reload, Dead }
    public DummyState currentState = DummyState.Idle;

    public float chaseRange = 15f;
    public float fireCooldown = 0.5f;

    [Header("Attack Range")]
    public float minAttackRange = 10f;
    public float maxAttackRange = 30f;

    private float attackRange;

    [SerializeField] private float fireTimer = 0f;

    //패트롤
    public float patrolRadius = 20f;
    private Vector3 originPosition;

    private NavMeshAgent agent;
    private DummyGunController gunController;
    private Player player;
    private Animator _animator;
    private HealthSystem _healthSystem;

    private static readonly int _Idle = Animator.StringToHash("Idle");
    private static readonly int _Chase = Animator.StringToHash("Chase");
    private static readonly int _Attack = Animator.StringToHash("Attack");
    private static readonly int _Patrol = Animator.StringToHash("Patrol");
    private static readonly int _Reload = Animator.StringToHash("Reload");
    private static readonly int _Dead = Animator.StringToHash("Dead");



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        gunController = GetComponent<DummyGunController>();
        _animator = GetComponent<Animator>();
        _healthSystem = GetComponent<HealthSystem>();

        
    }
    private void Start()
    {
        player = GameManager.Instance.player;
        gunController.Init(player);
        //스폰되자마자 사거리 설정
        attackRange = Random.Range(minAttackRange, maxAttackRange);

    }

    private void OnEnable()
    {
        _healthSystem.OnDeath.AddListener(OnDie);
    }
    private void Update()
    {
        FSMDummy();
    }


    private void FSMDummy()
    {
        /*if (currentState == DummyState.Dead)
        {
            agent.isStopped = true;
            _animator.SetTrigger(_Dead);
            return;
        }

        fireTimer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (currentState)
        {
            case DummyState.Idle:
                if (distanceToPlayer < chaseRange) ChangeState(DummyState.Chase);
                _animator.SetBool(_Idle, true);
                break;

            case DummyState.Patrol:
                Patrol();
                _animator.SetBool(_Patrol, true);
                if (distanceToPlayer < chaseRange) ChangeState(DummyState.Chase);
                break;

            case DummyState.Chase:
                agent.SetDestination(player.transform.position);
                _animator.SetBool(_Chase, true);
                if (distanceToPlayer < attackRange) ChangeState(DummyState.Attack);
                else if (distanceToPlayer > chaseRange) ChangeState(DummyState.Patrol);
                break;

            case DummyState.Attack:
                FaceTarget(player.transform.position);
                _animator.SetBool(_Attack, true);
                if (gunController.currentGun.currentAmmo == 0)
                {
                    ChangeState(DummyState.Reload);
                    return;
                }

                if (distanceToPlayer > attackRange)
                {
                    ChangeState(DummyState.Chase);
                    return;
                }

                if (distanceToPlayer <= attackRange && fireTimer >= fireCooldown)
                {
                    gunController.FireGun(); 
                    fireTimer = 0f;
                }
                break;

            case DummyState.Reload:
                if (gunController.ReloadWeapon())
                {
                    ChangeState(DummyState.Attack); //Reload 끝나면 다시 Attack
                }
                break;
        }*/
        fireTimer += Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (currentState == DummyState.Dead)
        {
            agent.isStopped = true;
            _animator.SetTrigger(_Dead);
            return;
        }

        //죽은 상태가 아니면 무조건 플레이어 Chase
        if (distanceToPlayer > attackRange)
        {
            //Chase
            if (currentState != DummyState.Chase)
                ChangeState(DummyState.Chase);

            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            _animator.SetBool(_Chase, true);
        }
        else
        {
            //사거리 안에 들어오면 공격 모드
            if (gunController.currentGun.currentAmmo == 0)
            {
                ChangeState(DummyState.Reload);
            }
            else
            {
                ChangeState(DummyState.Attack);
            }

            if (currentState == DummyState.Attack)
            {
                FaceTarget(player.transform.position);
                _animator.SetBool(_Attack, true);
                if (fireTimer >= fireCooldown)
                {
                    gunController.FireGun();
                    fireTimer = 0f;
                }
            }
            else if (currentState == DummyState.Reload)
            {
                if (gunController.ReloadWeapon())
                    ChangeState(DummyState.Attack);
            }
        }
    }

    //나중에 패트롤 봇도 추가
    private void Patrol()
    {
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            originPosition = transform.position;
            Vector3 randomPoint = GetRandomPointAroundOrigin();
            agent.SetDestination(randomPoint);
        }
    }
    private Vector3 GetRandomPointAroundOrigin()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += originPosition;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return hit.position;
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void ChangeState(DummyState newState)
    {
        if (currentState == DummyState.Attack)
        {
            agent.isStopped = false; //공격 상태에서 나올 때 이동 재개
        }

        currentState = newState;

        ChangeAnimation();


        if (currentState == DummyState.Attack)
        {
            agent.isStopped = true; //공격 상태로 진입할 때 이동 정지
        }
    }
    private void OnDie()
    {
        ChangeState(DummyState.Dead);
    }

    private void ChangeAnimation()
    {
        _animator.SetBool("Idle", false);
        _animator.SetBool("Chase", false);
        _animator.SetBool("Attack", false);
        _animator.SetBool("Reload", false);
        _animator.SetBool("Dead", false);
        _animator.SetBool("Patrol", false);

        switch (currentState)
        {
            case DummyState.Idle:
                _animator.SetBool("Idle", true);
                break;
            case DummyState.Patrol:
                _animator.SetBool("Patrol", true);
                break;
            case DummyState.Chase:
                _animator.SetBool("Chase", true);
                break;
            case DummyState.Attack:
                _animator.SetBool("Attack", true);
                agent.isStopped = true;
                break;
            case DummyState.Reload:
                _animator.SetBool("Reload", true);
                break;
            case DummyState.Dead:
                _animator.SetBool("Dead", true);
                break;
        }
    }


}

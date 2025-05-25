using UnityEngine;
using UnityEngine.Events;
public abstract class Gun : MonoBehaviour
{
    [Header("GunData")]
    public GunDataSO gunData;

    [Header("Camera and Ray")]
    public UnityEngine.Camera mainCamera;
    protected Ray ray;
    protected Vector3 targetPoint;
    public float maxDistance = 1000f;

    [Header("Fire Position")]
    [SerializeField] protected Transform fireTransform;

    [Header("Ads Position")]
    [SerializeField] public Transform adsTransform;

    private PlayerInputManager _input;
    private AudioSource _audioSource;

    [Header("Reload")]
    [SerializeField] public int currentAmmo;
    public bool isBulletAvailable = true;

    [Header("Animation")]
    public Animator animator;

    [Header("MuzzleFlash")]
    public ParticleSystem muzzleFlashPS;

    [Header("SFX")]
    public AudioClip oneShotClip;
    public AudioClip[] reloadClips;
    private int index = 0;

    //발사 재정전 이벤트
    public UnityEvent OnFireEvent;
    public UnityEvent OnReloadEvent;



    protected virtual void Start()
    {
        Init();
    }
    private void OnEnable()
    {
        CheckBullet();
    }
    protected virtual void Init()
    {
        _input = GetComponent<PlayerInputManager>();
        _audioSource = GetComponent<AudioSource>();
        if (TryGetComponent<Animator>(out Animator component))
        {
            animator = component;
        }
        mainCamera = UnityEngine.Camera.main;
        ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        currentAmmo = gunData.maxBulletCnt;
    }

    public virtual void CheckBullet()
    {
        isBulletAvailable = currentAmmo > 0;
        //_player.playerinventoryController.inventory.IsBulletAvailable(gunData.bulletType);
    }

    public bool TryFire()
    {
        CheckBullet();
        if (!isBulletAvailable)
        {
            return false;
        }
        FireGun();
        return true;
    }


    public void FireGun()
    {
        Fire();
        muzzleFlashPS.Play();
        _audioSource.PlayOneShot(oneShotClip);
        OnFireEvent?.Invoke();
    }

    public void PlayReloadClip()
    {
        ReloadClip(reloadClips);
    }

    public void ReloadClip(AudioClip[] audioclips)
    {
        if (_audioSource == null || audioclips == null) return;
        if (index >= audioclips.Length) index = 0;
        _audioSource.PlayOneShot(audioclips[index]);
        index += 1;
    }


    public abstract void Fire();
    
    //가상함수로 두어 재정의 할 수 있게함
    protected virtual void Shoot(Vector3 shootDir)
    {
        currentAmmo--;
        GameObject go = PoolManager.Instance.GetGameObject("Bullet");
        go.SetActive(true);
        go.transform.position = fireTransform.position;
        go.transform.rotation = Quaternion.LookRotation(shootDir);
    }

}






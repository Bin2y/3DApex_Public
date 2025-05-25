using UnityEngine;


public class Player : MonoBehaviour, IDamageable
{
    [Header("Components")]
    public HealthSystem healthSystem;
    public PlayerController playerController;
    public PlayerInputManager playerInputManager;
    public InteractionManager interactionManager;
    public PlayerEquipManager playerEquipManager;
    public GunController gunController;
    public PlayerInventoryController playerInventoryController;
    public PlayerUI playerUI;
    public PlayerItemController playerItemController;
    public FPSMovement fpsMovement;
    public FPSController fPSController;
    public Skill skill;




    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.Init(this);
        skill = GetComponent<Skill>();
        playerUI = GetComponent<PlayerUI>();
        playerUI.Init(this);
        playerController = GetComponent<PlayerController>();
        playerInputManager = GetComponent<PlayerInputManager>();
        interactionManager = GetComponent<InteractionManager>();
        playerEquipManager = GetComponent<PlayerEquipManager>();
        gunController = GetComponent<GunController>();
        playerInventoryController = GetComponent<PlayerInventoryController>();
        playerInventoryController.Init();
        playerItemController = GetComponent<PlayerItemController>();
        playerItemController.Init();
        fPSController = GetComponent<FPSController>();
        fpsMovement = GetComponent<FPSMovement>();
        
    }

    private void Start()
    {
        playerUI.inGameUI.ui_HPShieldBar.Init(healthSystem);
    }

    public bool ApplyDamage(DamageMessage damageMessage)
    {
        healthSystem.TakeDamage(damageMessage.amount);
        return true;
    }
}

using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Interact")]
    public float checkRate = 0.05f;
    [SerializeField] public float holdDuration = 1f;
    [SerializeField] private float interactionHoldTime = 0f;
    private bool isLookingAtInteractable = false;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;
    [Header("Interact Object")]
    private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    [Header("Prompt")]
    public GameObject itemPromptObject;
    public UI_Popup_ItemPrompt ui_popup_itemPrompt;

    private Camera _camera;
    private PlayerInputManager _playerInputManager;
    private Player _player;


    private void Start()
    {
        _camera = Camera.main;
        _playerInputManager = GetComponent<PlayerInputManager>();
        _player = GetComponent<Player>();
        ui_popup_itemPrompt = itemPromptObject.GetComponent<UI_Popup_ItemPrompt>();
        ui_popup_itemPrompt.Init();
    }

    private void Update()
    {
        Interact();
    }

    /// <summary>
    /// ����ĳ��Ʈ�� ������ ���� LateUpdate�� ���� ��
    /// </summary>
    private void LateUpdate()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            PerformRaycast();
        }
    }

    public void Interact()
    {
        if (isLookingAtInteractable && _playerInputManager.interact && curInteractable != null)
        {
            interactionHoldTime += Time.deltaTime;

            if (curInteractable.GetItemType() == Define.ItemType.weapon)
            {
                _player?.playerUI?.inGameUI?.ui_Interact.gameObject.SetActive(true);
                _player?.playerUI?.inGameUI?.ui_Interact?.PopupInteractUI(interactionHoldTime, holdDuration);
                if (interactionHoldTime >= holdDuration)
                {
                    curInteractable.OnInteractWeapon(_player);
                    interactionHoldTime = 0f;
                    _playerInputManager.interact = false;
                }
            }
            else
            {
                curInteractable.OnInteract(_player);
                interactionHoldTime = 0f;
                _playerInputManager.interact = false;
            }
        }
        else
        {
            _player?.playerUI?.inGameUI?.ui_Interact.gameObject.SetActive(false);
            interactionHoldTime = 0f; // ��ư�� ���� �ʱ�ȭ
        }
    }

    public void PerformRaycast()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
        {
            if (hit.collider.gameObject != curInteractGameObject)
            {
                curInteractGameObject = hit.collider.gameObject;
                curInteractable = hit.collider.GetComponent<IInteractable>();
                ui_popup_itemPrompt.SetPrompt(curInteractable.GetInteractPrompt());
                ui_popup_itemPrompt.dg_ui_fadeEffect.FadeIn();
            }

            // ���� ��ü�� �ٶ󺸰� �ִ� ����
            isLookingAtInteractable = curInteractable != null;
        }
        else
        {
            // Raycast ���� �� �ʱ�ȭ
            curInteractGameObject = null;
            curInteractable = null;
            interactionHoldTime = 0f;
            //�˾� ui Fade out
            ui_popup_itemPrompt?.dg_ui_fadeEffect?.FadeOut();
            isLookingAtInteractable = false; // �� �̻� ��ü�� ���� ���� ����
        }
    }
}

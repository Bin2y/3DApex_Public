
public class InventoryManager : Singleton<InventoryManager>
{
    public UI_Popup_Inventory inventory;

    protected override void Awake()
    {
        base.Awake();
        _isDontDestroyOnLoad = true;
    }


    /// <summary>
    /// 미리 인벤토리 캐싱해두기
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }
}
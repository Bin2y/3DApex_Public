
public class InventoryManager : Singleton<InventoryManager>
{
    public UI_Popup_Inventory inventory;

    protected override void Awake()
    {
        base.Awake();
        _isDontDestroyOnLoad = true;
    }


    /// <summary>
    /// �̸� �κ��丮 ĳ���صα�
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }
}
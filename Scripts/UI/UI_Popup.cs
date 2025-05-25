public class UI_Popup : UI_Base
{
    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.SetUIPopup(gameObject);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDestory()
    {
        base.OnDestory();
    }
}

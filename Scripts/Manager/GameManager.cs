using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public Player player { get; private set; }
    /// <summary>
    /// TODO : �ӽ÷� �÷��̾ ���ӸŴ����� ����
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _isDontDestroyOnLoad = true;
        player = FindObjectOfType<Player>();        
    }

    protected override void Start()
    {
        base.Start();
    }
}
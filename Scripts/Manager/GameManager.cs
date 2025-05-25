using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public Player player { get; private set; }
    /// <summary>
    /// TODO : 임시로 플레이어를 게임매니저로 관리
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
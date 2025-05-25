using System;
using UnityEngine;

public class UI_Scene : UI_Base
{
    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.SetUIScene(gameObject);
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
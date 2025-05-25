using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void OnEnable() { }
    protected void OnUIClickSound() { }
    protected virtual void OnDestory() { }
}
﻿// Designed by KINEMATION, 2025.

using UnityEngine;


public class ScopeAttachment : BaseAttachment
{
    [Min(0f)] public float aimFovZoom = 1f;
    [Range(0f, 2f)] public float sensitivityMultiplier = 1f;
    public Transform aimPoint;
}

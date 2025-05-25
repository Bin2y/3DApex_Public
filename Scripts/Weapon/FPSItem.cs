// Designed by KINEMATION, 2025.

using KINEMATION.FPSAnimationFramework.Runtime.Layers.IkMotionLayer;
using UnityEngine;


public abstract class FPSItem : MonoBehaviour
{
    [SerializeField] protected RuntimeAnimatorController overrideController;

    [SerializeField] protected IkMotionLayerSettings equipMotion;
    [SerializeField] protected IkMotionLayerSettings unEquipMotion;

    [SerializeField] public Define.WeaponType weaponType;
    public virtual void OnEquip(GameObject parent) { }

    public virtual void OnUnEquip() { }

    public virtual bool OnAimPressed() { return false; }

    public virtual bool OnAimReleased() { return false; }

    public virtual bool OnFirePressed() { return false; }

    public virtual bool OnFireReleased() { return false; }

    public virtual bool OnReload() { return false; }

    public virtual bool OnGrenadeThrow() { return false; }

    public virtual void OnCycleScope() { }

    public virtual void OnChangeFireMode() { }

    public virtual void OnAttachmentChanged(int attachmentTypeIndex) { }

    public virtual Define.WeaponType OnGetWeaponType() { return weaponType; }
}

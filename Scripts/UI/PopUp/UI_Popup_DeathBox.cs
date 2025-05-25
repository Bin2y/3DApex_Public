using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Popup_DeathBox : UI_Popup
{
    [Header("DeathBox UI")]
    public Image characterIcon;
    public TextMeshProUGUI characterName;

    [Header("Slots")]
    [SerializeField] private List<UI_SubItem_DeathBox_WeaponSlot> _weaponSlots;
    [SerializeField] private List<UI_SubItem_DeathBox_BulletSlot> _bulletSlots;
    [SerializeField] private List<UI_SubItem_DeathBox_ItemSlot> _itemSlots;

    [Header("Close Button")]
    [SerializeField] private Button closeButton;

    private Coroutine autoCloseCoroutine;

    private DeathBox linkedDeathBox;
    public Player player;


    public void Bind(Player player, DeathBox deathBox)
    {
        this.player = player;

        //SubItem ui들이 본체를 가지도록함
        foreach (var slot in _bulletSlots) slot.parentUI = this;
        foreach (var slot in _weaponSlots) slot.parentUI = this;
        foreach (var slot in _itemSlots) slot.parentUI = this;
        this.linkedDeathBox = deathBox;
        Refresh();

        ToggleUI(true);

        if (autoCloseCoroutine != null) { StopCoroutine(autoCloseCoroutine); }
        autoCloseCoroutine = StartCoroutine(AutoCloseByDistance());
    }

    private IEnumerator AutoCloseByDistance()
    {
        while (true)
        {
            float distSqr = (player.transform.position - linkedDeathBox.transform.position).sqrMagnitude;
            if (distSqr > linkedDeathBox.closeDistance)
            {
                ToggleUI(false);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 모델 변경 시 ui 전체 리프레시
    /// </summary>
    public void Refresh()
    {
        PopulateWeapons();
        PopulateBullets();
        PopulateItems();
    }



    private void PopulateBullets()
    {
        for (int i = 0; i < _bulletSlots.Count; i++)
        {
            if (i < linkedDeathBox.bullets.Count)
                _bulletSlots[i].Bind(linkedDeathBox, ItemCategory.Bullet, i);
            else
                _bulletSlots[i].Clear();
        }
    }

    private void PopulateWeapons()
    {
        for (int i = 0; i < _weaponSlots.Count; i++)
            if (i < linkedDeathBox.weapons.Count)
                _weaponSlots[i].Bind(linkedDeathBox, ItemCategory.Weapon, i);
            else
                _weaponSlots[i].Clear();
    }

    private void PopulateItems()
    {
        for (int i = 0; i < _itemSlots.Count; i++)
            if (i < linkedDeathBox.items.Count)
                _itemSlots[i].Bind(linkedDeathBox, ItemCategory.Inventory, i);
            else
                _itemSlots[i].Clear();
    }

    public void ToggleUI(bool isActive)
    {
        gameObject.SetActive(isActive);
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        player.playerInputManager.IsLookLock(isActive);
    }


}

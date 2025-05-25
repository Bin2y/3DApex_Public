using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SubItem_Grenade : UI_SubItem
{
    private Player _player;
    [SerializeField] private List<Sprite> images;
    public Image grenadeIcon;

    public void Init(Player player)
    {
        _player = player;
    }

    public void UpdateUI(int idx)
    {
        Color color = Color.white;
        color.a = 255;
        grenadeIcon.sprite = images[idx];
        grenadeIcon.color = color;
    }
}



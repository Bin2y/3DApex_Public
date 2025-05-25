using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_SubItem_Heal : UI_SubItem
{
    private Player _player;
    [SerializeField] private List<Sprite> images;
    public Image healIcon;

    public void Init(Player player)
    {
        _player = player;
    }

    public void UpdateUI(int idx)
    {
        Color color = Color.white;
        color.a = 255;
        healIcon.sprite = images[idx];
        healIcon.color = color;
    }
}



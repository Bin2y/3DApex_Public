using UnityEngine;
public static class UnityExtension
{
    public static Color HexColor(string hexCode)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexCode, out color))
        {
            return color;
        }

        //Debug.LogError("Failed Invaild to hexCode " + hexCode);
        return Color.white;
    }
}
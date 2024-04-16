using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Palette
{
    private static List<Color32> colors = null;

    public static int LastColorIndex { get { return GetDefaultColors().Count - 1; } }

    public static byte DefaultColorIndex { get { return (byte)LastColorIndex; } }
    

    public static Color32 GetColor(int index)
    {
        try
        {
            return GetDefaultColors()[index];
        }
        catch(System.Exception e)
        {
            Debug.Log("Index of color incorrect\n" + e);
        }
        return Color.white;
    }

    public static Color32 GetDefaultColor()
    {
        return GetDefaultColors()[DefaultColorIndex];
    }

    public static List<Color32> GetDefaultColors()
    {
        if(colors == null)
        {
            colors = ExtractColorsFromImage();
        }

        return colors;
    }

    private static List<Color32> ExtractColorsFromImage()
    {
        List<Color32> temp = new List<Color32>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("palette");

        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite s = sprites[i];
            Vector2 pivot = s.pivot;
            Vector2 offset = s.rect.position;
            
            int x = Mathf.FloorToInt(pivot.x + offset.x);
            int y = Mathf.FloorToInt(pivot.y + offset.y);

            temp.Add(s.texture.GetPixel(x, y));
            //Debug.Log(i + " : " + temp[i] + " original one: " + s.texture.GetPixel(x, y));
            //Debug.Log(offset.x + ", " + offset.y);
        }

        return temp;
    }
}
